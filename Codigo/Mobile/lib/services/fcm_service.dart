import 'dart:async';
import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import '../config/app_mocks.dart';
import 'database_service.dart';

@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  try {
    await Firebase.initializeApp();
  } catch (_) {}

  await FcmService.processarMensagemSegundoPlano(message.data);
}

class FcmService {
  static FirebaseMessaging get _firebaseMessaging => FirebaseMessaging.instance;
  static final _localNotifications = FlutterLocalNotificationsPlugin();
  static Future<void>? _initFuture;
  static bool _localNotificationsInitialized = false;
  static Timer? _timerAlarmeLocal;
  static Timer? _timerTimeoutAlarme;

  static const int idNotificacaoAlarme = 999;
  static const String tipoSincronizarPlanejamentos = 'SINCRONIZAR_PLANEJAMENTOS';
  static const String tipoLembreteMedicamento = 'LEMBRETE_MEDICAMENTO';

  static const AndroidNotificationSound _somPadraoAlarmeDispositivo =
      UriAndroidNotificationSound('content://settings/system/alarm_alert');

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
  static VoidCallback? onAlarmeDisparado;
  static VoidCallback? onAlarmeSilenciadoAutomaticamente;

  static Future<void> inicializar() {
    _initFuture ??= _executarInicializacao();
    return _initFuture!;
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
      onDidReceiveNotificationResponse: (NotificationResponse response) {
        if (response.payload == 'ALARME') {
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
        await mostrarNotificacaoBanner(
          id: notification.hashCode,
          titulo: notification?.title ?? NotificacaoMock.tituloSincronizacao,
          corpo: notification?.body ?? NotificacaoMock.corpoSincronizacao,
          payload: 'SINCRONIZACAO',
        );

        await processarMensagemSegundoPlano(message.data);
      });

      FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) async {
        await processarMensagemSegundoPlano(message.data);
      });
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
    String titulo = NotificacaoMock.tituloAlarme,
    String corpo = NotificacaoMock.corpoAlarme,
  }) async {
    await _inicializarNotificacoesLocais();
    onAlarmeDisparado?.call();

    _timerTimeoutAlarme?.cancel();
    _timerTimeoutAlarme = Timer(
      const Duration(seconds: NotificacaoMock.duracaoMaximaAlarmeSegundos),
      () async {
        await pararAlarmeMedicamento();
        onAlarmeSilenciadoAutomaticamente?.call();
      },
    );

    if (!_localNotificationsInitialized) {
      debugPrint('[FcmService] Alarme disparado: $titulo - $corpo');
      return;
    }

    await _localNotifications.show(
      id: idNotificacaoAlarme,
      title: titulo,
      body: corpo,
      payload: 'ALARME',
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

  static Future<void> pararAlarmeMedicamento() async {
    _timerAlarmeLocal?.cancel();
    _timerTimeoutAlarme?.cancel();
    if (_localNotificationsInitialized) {
      await _localNotifications.cancel(id: idNotificacaoAlarme);
    }
  }

  static Future<void> processarMensagemSegundoPlano(
    Map<String, dynamic> data,
  ) async {
    debugPrint(
      '[FcmService] Sincronizando planejamentos no SQLite e agendando alarme local...',
    );
    await DatabaseService().sincronizarPlanejamentos();
    onSincronizacaoRecebida?.call(data);

    _timerAlarmeLocal?.cancel();
    debugPrint(
      '[FcmService] Alarme local agendado para disparar em ${NotificacaoMock.segundosParaAlarmeTeste} segundos!',
    );
    _timerAlarmeLocal = Timer(
      const Duration(seconds: NotificacaoMock.segundosParaAlarmeTeste),
      () {
        dispararAlarmeMedicamento();
      },
    );
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
