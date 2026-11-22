import 'dart:async';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:timezone/data/latest.dart' as tz_data;
import 'package:timezone/timezone.dart' as tz;
import '../config/app_mocks.dart';
import '../dtos/planejamento_dto.dart';
import 'planejamento_service.dart';

@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  WidgetsFlutterBinding.ensureInitialized();
  try {
    await Firebase.initializeApp();
  } catch (_) {}

  if (message.data.isNotEmpty) {
    await FcmService.processarMensagemSegundoPlano(
      message.data,
      emBackground: true,
    );
  }
}

class FcmService {
  static FirebaseMessaging get _firebaseMessaging => FirebaseMessaging.instance;
  static final _localNotifications = FlutterLocalNotificationsPlugin();
  static Future<void>? _initFuture;
  static bool _localNotificationsInitialized = false;
  static bool _timezonesInitialized = false;
  static Timer? _timerAlarmeLocal;
  static Timer? _timerTimeoutAlarme;
  static final List<Timer> _timersHorariosAgendados = [];
  static final Set<String> _chavesDisparadas = {};

  static const String _keyAlarmePendente = 'alarme_pendente_ativo';
  static const String _keyIdPlanejamentoAlarme = 'id_planejamento_alarme_ativo';
  static const String _uriSomAlarmePadrao =
      'content://settings/system/alarm_alert';
  static const String tipoSincronizarPlanejamentos = 'SINCRONIZAR_PLANEJAMENTOS';
  static const String tipoLembreteMedicamento = 'LEMBRETE_MEDICAMENTO';

  static const AndroidNotificationSound _somPadraoAlarmeDispositivo =
      UriAndroidNotificationSound(_uriSomAlarmePadrao);

  static const AndroidNotificationChannel _syncChannel =
      AndroidNotificationChannel(
    'medica_sync_channel',
    'Atualizações de Planejamento',
    description: 'Avisos quando o cuidador altera os horários de medicamentos.',
    importance: Importance.high,
    playSound: true,
  );

  static const AndroidNotificationChannel _alarmChannel =
      AndroidNotificationChannel(
    'medica_alarm_channel_v4',
    'Alarme de Medicamentos',
    description: 'Alarme sonoro quando chega o horário do medicamento.',
    importance: Importance.max,
    playSound: true,
    sound: _somPadraoAlarmeDispositivo,
    enableVibration: true,
    audioAttributesUsage: AudioAttributesUsage.alarm,
  );

  static void Function(String? payload)? onNotificacaoClicada;
  static void Function(Map<String, dynamic> data)? onSincronizacaoRecebida;
  static void Function(int? idPlanejamento)? onAlarmeDisparado;
  static VoidCallback? onAlarmeSilenciadoAutomaticamente;

  static Future<void> inicializar() {
    _initFuture ??= _executarInicializacao();
    return _initFuture!;
  }

  static void _garantirTimezones() {
    if (_timezonesInitialized) return;
    tz_data.initializeTimeZones();
    _timezonesInitialized = true;
  }

  static NotificationDetails _detalhesNotificacaoAlarme() {
    return NotificationDetails(
      android: AndroidNotificationDetails(
        _alarmChannel.id,
        _alarmChannel.name,
        channelDescription: _alarmChannel.description,
        icon: '@mipmap/ic_launcher',
        priority: Priority.max,
        importance: Importance.max,
        playSound: true,
        sound: _somPadraoAlarmeDispositivo,
        fullScreenIntent: true,
        ongoing: true,
        autoCancel: false,
        onlyAlertOnce: false,
        timeoutAfter: NotificacaoMock.duracaoMaximaAlarmeSegundos * 1000,
        category: AndroidNotificationCategory.alarm,
        audioAttributesUsage: AudioAttributesUsage.alarm,
        additionalFlags: Int32List.fromList([4]),
      ),
      iOS: const DarwinNotificationDetails(
        presentAlert: true,
        presentBadge: true,
        presentSound: true,
      ),
    );
  }

  static Future<void> _mostrarNotificacaoSomAlarme({
    required int idPlanejamento,
    String titulo = NotificacaoMock.tituloAlarme,
    String corpo = NotificacaoMock.corpoAlarme,
  }) async {
    await _inicializarNotificacoesLocais();
    if (!_localNotificationsInitialized) return;

    await _localNotifications.show(
      id: idPlanejamento,
      title: titulo,
      body: corpo,
      payload: '$idPlanejamento',
      notificationDetails: _detalhesNotificacaoAlarme(),
    );
  }

  static Future<void> retomarSomAlarme() async {
    final prefs = await SharedPreferences.getInstance();
    final idPlanejamento = prefs.getInt(_keyIdPlanejamentoAlarme) ?? 1;
    await _mostrarNotificacaoSomAlarme(idPlanejamento: idPlanejamento);
  }

  static Future<void> verificarAlarmePendenteAoAbrir() async {
    await _inicializarNotificacoesLocais();

    final prefs = await SharedPreferences.getInstance();
    await prefs.reload();
    final pendente = prefs.getBool(_keyAlarmePendente) ?? false;
    int? idAlarme = prefs.getInt(_keyIdPlanejamentoAlarme);

    final launchDetails =
        await _localNotifications.getNotificationAppLaunchDetails();
    final payloadLaunch = launchDetails?.notificationResponse?.payload;
    final abriuPeloAlarme =
        (launchDetails?.didNotificationLaunchApp ?? false) &&
            payloadLaunch != null &&
            payloadLaunch.isNotEmpty &&
            payloadLaunch != 'SINCRONIZACAO';

    if (abriuPeloAlarme) {
      idAlarme = int.tryParse(payloadLaunch) ?? idAlarme;
    }

    bool temNotificacaoAlarmeAtiva = false;
    if (!kIsWeb && defaultTargetPlatform == TargetPlatform.android) {
      try {
        final androidPlugin = _localNotifications
            .resolvePlatformSpecificImplementation<
                AndroidFlutterLocalNotificationsPlugin>();
        final ativas = await androidPlugin?.getActiveNotifications() ?? [];
        for (final n in ativas) {
          if (n.channelId == _alarmChannel.id && n.id != null) {
            temNotificacaoAlarmeAtiva = true;
            idAlarme = n.id;
            break;
          }
        }
      } catch (_) {}
    }

    if (pendente || abriuPeloAlarme || temNotificacaoAlarmeAtiva) {
      if (idAlarme != null) {
        await prefs.setBool(_keyAlarmePendente, true);
        await prefs.setInt(_keyIdPlanejamentoAlarme, idAlarme);
      }
      onAlarmeDisparado?.call(idAlarme);
      await retomarSomAlarme();
    }
  }

  static Future<void> _inicializarNotificacoesLocais() async {
    _garantirTimezones();
    if (_localNotificationsInitialized) return;
    if (kIsWeb) return;
    if (defaultTargetPlatform != TargetPlatform.android &&
        defaultTargetPlatform != TargetPlatform.iOS) {
      return;
    }

    const initSettings = InitializationSettings(
      android: AndroidInitializationSettings('@mipmap/ic_launcher'),
      iOS: DarwinInitializationSettings(
        requestAlertPermission: true,
        requestBadgePermission: true,
        requestSoundPermission: true,
      ),
    );

    await _localNotifications.initialize(
      settings: initSettings,
      onDidReceiveNotificationResponse: (NotificationResponse response) async {
        final payload = response.payload;
        if (payload != null &&
            payload.isNotEmpty &&
            payload != 'SINCRONIZACAO') {
          final idAlarme = int.tryParse(payload);
          if (idAlarme != null) {
            final prefs = await SharedPreferences.getInstance();
            await prefs.setBool(_keyAlarmePendente, true);
            await prefs.setInt(_keyIdPlanejamentoAlarme, idAlarme);
          }
          onAlarmeDisparado?.call(idAlarme);
          await retomarSomAlarme();
        }
        onNotificacaoClicada?.call(payload);
      },
    );

    final androidPlugin = _localNotifications
        .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>();
    await androidPlugin?.createNotificationChannel(_syncChannel);
    await androidPlugin?.createNotificationChannel(_alarmChannel);
    await androidPlugin?.requestNotificationsPermission();

    _localNotificationsInitialized = true;
  }

  static Future<void> _executarInicializacao() async {
    if (kIsWeb) return;
    if (defaultTargetPlatform != TargetPlatform.android &&
        defaultTargetPlatform != TargetPlatform.iOS) {
      return;
    }

    try {
      await _inicializarNotificacoesLocais();
    } catch (e) {
      debugPrint('Erro ao inicializar notificações locais: $e');
    }

    try {
      await Firebase.initializeApp();

      await _firebaseMessaging.requestPermission(
        alert: true,
        badge: true,
        sound: true,
      );

      FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);

      FirebaseMessaging.onMessage.listen((RemoteMessage message) async {
        final notification = message.notification;
        if (notification != null) {
          await mostrarNotificacaoBanner(
            id: notification.hashCode,
            titulo: notification.title ?? NotificacaoMock.tituloSincronizacao,
            corpo: notification.body ?? NotificacaoMock.corpoSincronizacao,
            payload: 'SINCRONIZACAO',
          );
        }

        if (message.data.isNotEmpty) {
          await processarMensagemSegundoPlano(message.data);
        }
      });

      FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) async {
        await processarMensagemSegundoPlano(message.data);
      });

      final initialMessage = await _firebaseMessaging.getInitialMessage();
      if (initialMessage != null) {
        await processarMensagemSegundoPlano(initialMessage.data);
      }
    } catch (e) {
      debugPrint('Erro ao inicializar FcmService: $e');
    }
  }

  static Future<void> mostrarNotificacaoBanner({
    int? id,
    required String titulo,
    required String corpo,
    String? payload,
  }) async {
    await _inicializarNotificacoesLocais();

    if (!_localNotificationsInitialized) {
      debugPrint('[FcmService] Banner simulado: $titulo - $corpo');
      return;
    }

    await _localNotifications.show(
      id: id ?? DateTime.now().millisecondsSinceEpoch.remainder(100000),
      title: titulo,
      body: corpo,
      payload: payload,
      notificationDetails: NotificationDetails(
        android: AndroidNotificationDetails(
          _syncChannel.id,
          _syncChannel.name,
          channelDescription: _syncChannel.description,
          icon: '@mipmap/ic_launcher',
          priority: Priority.high,
          importance: Importance.high,
          ticker: titulo,
        ),
        iOS: const DarwinNotificationDetails(
          presentAlert: true,
          presentBadge: true,
          presentSound: true,
        ),
      ),
    );
  }

  static Future<void> dispararAlarmeMedicamento({
    required int idPlanejamento,
    String titulo = NotificacaoMock.tituloAlarme,
    String corpo = NotificacaoMock.corpoAlarme,
  }) async {
    await _inicializarNotificacoesLocais();

    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool(_keyAlarmePendente, true);
    await prefs.setInt(_keyIdPlanejamentoAlarme, idPlanejamento);

    onAlarmeDisparado?.call(idPlanejamento);

    _timerTimeoutAlarme?.cancel();
    _timerTimeoutAlarme = Timer(
      const Duration(seconds: NotificacaoMock.duracaoMaximaAlarmeSegundos),
      () async {
        await pararSomAlarmeMedicamento();
        onAlarmeSilenciadoAutomaticamente?.call();
      },
    );

    await _mostrarNotificacaoSomAlarme(
      idPlanejamento: idPlanejamento,
      titulo: titulo,
      corpo: corpo,
    );
  }

  static Future<void> pararSomAlarmeMedicamento() async {
    _timerAlarmeLocal?.cancel();
    _timerTimeoutAlarme?.cancel();
    if (_localNotificationsInitialized) {
      final prefs = await SharedPreferences.getInstance();
      final idPlanejamento = prefs.getInt(_keyIdPlanejamentoAlarme) ?? 1;
      await _localNotifications.cancel(id: idPlanejamento);
    }
  }

  static Future<void> pararAlarmeMedicamento() async {
    await pararSomAlarmeMedicamento();
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool(_keyAlarmePendente, false);
    await prefs.remove(_keyIdPlanejamentoAlarme);
  }

  static bool _isDiaPlanejado(String diaSemana, DateTime data) {
    final limpo = diaSemana.trim().toUpperCase();
    if (limpo.isEmpty) return true;

    final indexDia = data.weekday % 7;
    if (limpo.length == 7 && !limpo.contains(',')) {
      return limpo[indexDia] != 'X';
    }

    const siglas = ['DOM', 'SEG', 'TER', 'QUA', 'QUI', 'SEX', 'SAB'];
    final partes = limpo.split(',').map((s) => s.trim()).toList();
    return partes.contains(siglas[indexDia]);
  }

  static DateTime? _calcularProximoHorario(
    PlanejamentoResponseDto item,
    DateTime agora,
  ) {
    final partes = item.hora.split(':');
    if (partes.length < 2) return null;

    final h = int.tryParse(partes[0]);
    final m = int.tryParse(partes[1]);
    if (h == null || m == null) return null;

    final inicioDia = DateTime(
      item.dataInicio.year,
      item.dataInicio.month,
      item.dataInicio.day,
    );
    final fimDia = item.dataFim != null && item.dataFim!.year <= 9000
        ? DateTime(item.dataFim!.year, item.dataFim!.month, item.dataFim!.day)
        : null;

    for (int offset = 0; offset <= 7; offset++) {
      final diaCandidato = DateTime(
        agora.year,
        agora.month,
        agora.day,
      ).add(Duration(days: offset));

      if (diaCandidato.isBefore(inicioDia)) continue;
      if (fimDia != null && diaCandidato.isAfter(fimDia)) return null;
      if (!_isDiaPlanejado(item.diaSemana, diaCandidato)) continue;

      final horarioCandidato = DateTime(
        diaCandidato.year,
        diaCandidato.month,
        diaCandidato.day,
        h,
        m,
      );

      if (horarioCandidato.isAfter(agora)) {
        return horarioCandidato;
      }
    }

    return null;
  }

  static Future<void> agendarHorariosDosPlanejamentos(
    List<PlanejamentoResponseDto> planejamentos,
  ) async {
    await _inicializarNotificacoesLocais();

    for (final timer in _timersHorariosAgendados) {
      timer.cancel();
    }
    _timersHorariosAgendados.clear();

    if (_localNotificationsInitialized) {
      try {
        await _localNotifications.cancelAllPendingNotifications();
      } catch (e) {
        debugPrint('[FcmService] Erro ao cancelar pendentes: $e');
      }
    }

    final agora = DateTime.now();

    for (final item in planejamentos) {
      final partes = item.hora.split(':');
      if (partes.length < 2) continue;

      final h = int.tryParse(partes[0]);
      final m = int.tryParse(partes[1]);
      if (h == null || m == null) continue;

      final chaveMinuto =
          '${item.id}_${agora.year}-${agora.month}-${agora.day}_$h:$m';

      final inicioDia = DateTime(
        item.dataInicio.year,
        item.dataInicio.month,
        item.dataInicio.day,
      );
      final hojeDia = DateTime(agora.year, agora.month, agora.day);
      final validoHoje = !hojeDia.isBefore(inicioDia) &&
          _isDiaPlanejado(item.diaSemana, hojeDia);

      if (validoHoje && agora.hour == h && agora.minute == m) {
        if (!_chavesDisparadas.contains(chaveMinuto)) {
          _chavesDisparadas.add(chaveMinuto);
          await dispararAlarmeMedicamento(idPlanejamento: item.id);
        }
        continue;
      }

      final proximoHorario = _calcularProximoHorario(item, agora);
      if (proximoHorario == null) continue;

      final horaFormatada =
          '${h.toString().padLeft(2, '0')}h${m.toString().padLeft(2, '0')}';

      if (_localNotificationsInitialized) {
        final tzHorario = tz.TZDateTime.from(proximoHorario.toUtc(), tz.UTC);
        try {
          await _localNotifications.zonedSchedule(
            id: item.id,
            title: NotificacaoMock.tituloAlarme,
            body: 'Hora do medicamento ($horaFormatada).',
            scheduledDate: tzHorario,
            notificationDetails: _detalhesNotificacaoAlarme(),
            androidScheduleMode: AndroidScheduleMode.alarmClock,
            payload: '${item.id}',
          );
          debugPrint(
            '[FcmService] Alarme nativo agendado (id=${item.id}) para $proximoHorario',
          );
        } catch (e) {
          debugPrint('[FcmService] Fallback exactAllowWhileIdle (id=${item.id}): $e');
          try {
            await _localNotifications.zonedSchedule(
              id: item.id,
              title: NotificacaoMock.tituloAlarme,
              body: 'Hora do medicamento ($horaFormatada).',
              scheduledDate: tzHorario,
              notificationDetails: _detalhesNotificacaoAlarme(),
              androidScheduleMode: AndroidScheduleMode.exactAllowWhileIdle,
              payload: '${item.id}',
            );
          } catch (e2) {
            debugPrint('[FcmService] Erro ao agendar alarme id=${item.id}: $e2');
          }
        }
      }

      final espera = proximoHorario.difference(agora);
      if (espera <= const Duration(hours: 24)) {
        final chaveAgendada =
            '${item.id}_${proximoHorario.year}-${proximoHorario.month}-${proximoHorario.day}_$h:$m';
        final timer = Timer(espera, () async {
          if (!_chavesDisparadas.contains(chaveAgendada)) {
            _chavesDisparadas.add(chaveAgendada);
            final prefs = await SharedPreferences.getInstance();
            await prefs.setBool(_keyAlarmePendente, true);
            await prefs.setInt(_keyIdPlanejamentoAlarme, item.id);
            onAlarmeDisparado?.call(item.id);
          }
        });
        _timersHorariosAgendados.add(timer);
      }
    }
  }

  static Future<void> processarMensagemSegundoPlano(
    Map<String, dynamic> data, {
    bool emBackground = false,
    bool simularEm5Segundos = false,
  }) async {
    debugPrint(
      '[FcmService] Sincronizando planejamentos no SQLite e agendando alarmes nativos...',
    );
    final planejamentos = await PlanejamentoService().sincronizarPlanejamentos();
    onSincronizacaoRecebida?.call(data);

    await agendarHorariosDosPlanejamentos(planejamentos);

    if (simularEm5Segundos) {
      final idPlanejamento =
          planejamentos.isNotEmpty ? planejamentos.last.id : 1;
      _timerAlarmeLocal?.cancel();

      if (emBackground) {
        await Future.delayed(
          const Duration(seconds: NotificacaoMock.segundosParaAlarmeTeste),
        );
        await dispararAlarmeMedicamento(idPlanejamento: idPlanejamento);
      } else {
        _timerAlarmeLocal = Timer(
          const Duration(seconds: NotificacaoMock.segundosParaAlarmeTeste),
          () {
            dispararAlarmeMedicamento(idPlanejamento: idPlanejamento);
          },
        );
      }
    }
  }

  static Future<String?> getToken() async {
    try {
      if (kIsWeb) {
        debugPrint(
          'FcmService: Plataforma Web detectada (FCM nativo não suportado sem VAPID)',
        );
        return null;
      }
      await inicializar();
      final token = await _firebaseMessaging.getToken();
      debugPrint('FcmService: Token FCM real obtido do aparelho: $token');
      return token;
    } catch (e) {
      debugPrint('FcmService: Erro ao obter Token FCM do Firebase: $e');
      return null;
    }
  }
}
