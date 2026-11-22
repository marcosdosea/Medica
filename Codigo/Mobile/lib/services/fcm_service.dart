import 'dart:async';
import 'package:audioplayers/audioplayers.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../config/app_mocks.dart';
import 'planejamento_service.dart';

@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  WidgetsFlutterBinding.ensureInitialized();
  try {
    await Firebase.initializeApp();
  } catch (_) {}

  if (message.notification == null) {
    final titulo =
        message.data['titulo']?.toString() ?? NotificacaoMock.tituloSincronizacao;
    final corpo =
        message.data['corpo']?.toString() ?? NotificacaoMock.corpoSincronizacao;
    await FcmService.mostrarNotificacaoBanner(
      titulo: titulo,
      corpo: corpo,
      payload: 'SINCRONIZACAO',
    );
  }

  await FcmService.processarMensagemSegundoPlano(
    message.data,
    emBackground: true,
  );
}

class FcmService {
  static FirebaseMessaging get _firebaseMessaging => FirebaseMessaging.instance;
  static final _localNotifications = FlutterLocalNotificationsPlugin();
  static final AudioPlayer _alarmePlayer = AudioPlayer();
  static Future<void>? _initFuture;
  static bool _localNotificationsInitialized = false;
  static Timer? _timerAlarmeLocal;
  static Timer? _timerTimeoutAlarme;

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
    enableVibration: false,
    audioAttributesUsage: AudioAttributesUsage.alarm,
  );

  static void Function(String? payload)? onNotificacaoClicada;
  static void Function(Map<String, dynamic> data)? onSincronizacaoRecebida;
  static VoidCallback? onAlarmeDisparado;
  static VoidCallback? onAlarmeSilenciadoAutomaticamente;

  static Future<void> inicializar() {
    _initFuture ??= _executarInicializacao();
    return _initFuture!;
  }

  static Future<void> _iniciarToqueAlarmeContinuo() async {
    try {
      await _alarmePlayer.setAudioContext(
        AudioContext(
          android: const AudioContextAndroid(
            isSpeakerphoneOn: true,
            stayAwake: true,
            contentType: AndroidContentType.sonification,
            usageType: AndroidUsageType.alarm,
            audioFocus: AndroidAudioFocus.gainTransient,
          ),
        ),
      );
      await _alarmePlayer.setReleaseMode(ReleaseMode.loop);
      await _alarmePlayer.play(UrlSource(_uriSomAlarmePadrao));
    } catch (e) {
      debugPrint('Erro ao iniciar toque contínuo do alarme: $e');
    }
  }

  static Future<void> verificarAlarmePendenteAoAbrir() async {
    await _inicializarNotificacoesLocais();

    final prefs = await SharedPreferences.getInstance();
    await prefs.reload();
    final pendente = prefs.getBool(_keyAlarmePendente) ?? false;

    final launchDetails =
        await _localNotifications.getNotificationAppLaunchDetails();
    final payloadLaunch = launchDetails?.notificationResponse?.payload;
    final abriuPeloAlarme =
        (launchDetails?.didNotificationLaunchApp ?? false) &&
            payloadLaunch != null &&
            payloadLaunch != 'SINCRONIZACAO';

    if (pendente || abriuPeloAlarme) {
      await _iniciarToqueAlarmeContinuo();
      onAlarmeDisparado?.call();
    }
  }

  static Future<void> _inicializarNotificacoesLocais() async {
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
        if (response.payload != null && response.payload != 'SINCRONIZACAO') {
          await _iniciarToqueAlarmeContinuo();
          onAlarmeDisparado?.call();
        }
        onNotificacaoClicada?.call(response.payload);
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
        final titulo = notification?.title ??
            message.data['titulo']?.toString() ??
            NotificacaoMock.tituloSincronizacao;
        final corpo = notification?.body ??
            message.data['corpo']?.toString() ??
            NotificacaoMock.corpoSincronizacao;
        await mostrarNotificacaoBanner(
          id: notification?.hashCode,
          titulo: titulo,
          corpo: corpo,
          payload: 'SINCRONIZACAO',
        );

        await processarMensagemSegundoPlano(message.data);
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

    await _iniciarToqueAlarmeContinuo();
    onAlarmeDisparado?.call();

    _timerTimeoutAlarme?.cancel();
    _timerTimeoutAlarme = Timer(
      const Duration(seconds: NotificacaoMock.duracaoMaximaAlarmeSegundos),
      () async {
        await pararSomAlarmeMedicamento();
        onAlarmeSilenciadoAutomaticamente?.call();
      },
    );

    if (!_localNotificationsInitialized) {
      debugPrint('[FcmService] Alarme disparado: $titulo - $corpo');
      return;
    }

    await _localNotifications.show(
      id: idPlanejamento,
      title: titulo,
      body: corpo,
      payload: '$idPlanejamento',
      notificationDetails: NotificationDetails(
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
      ),
    );
  }

  static Future<void> pararSomAlarmeMedicamento() async {
    _timerAlarmeLocal?.cancel();
    _timerTimeoutAlarme?.cancel();
    try {
      await _alarmePlayer.stop();
    } catch (_) {}
    if (_localNotificationsInitialized) {
      final prefs = await SharedPreferences.getInstance();
      final idPlanejamento = prefs.getInt(_keyIdPlanejamentoAlarme);
      if (idPlanejamento != null) {
        await _localNotifications.cancel(id: idPlanejamento);
      }
    }
  }

  static Future<void> pararAlarmeMedicamento() async {
    await pararSomAlarmeMedicamento();
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool(_keyAlarmePendente, false);
    await prefs.remove(_keyIdPlanejamentoAlarme);
  }

  static Future<void> processarMensagemSegundoPlano(
    Map<String, dynamic> data, {
    bool emBackground = false,
  }) async {
    debugPrint(
      '[FcmService] Sincronizando planejamentos no SQLite e agendando alarme local...',
    );
    final planejamentos = await PlanejamentoService().sincronizarPlanejamentos();
    onSincronizacaoRecebida?.call(data);

    if (planejamentos.isEmpty) return;
    final idPlanejamento = planejamentos.last.id;

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
