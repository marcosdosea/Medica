import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp();
}

class FcmService {
  static FirebaseMessaging get _firebaseMessaging => FirebaseMessaging.instance;
  static final _localNotifications = FlutterLocalNotificationsPlugin();
  static Future<void>? _initFuture;

  static Future<void> inicializar() {
    _initFuture ??= _executarInicializacao();
    return _initFuture!;
  }

  static Future<void> _executarInicializacao() async {
    if (kIsWeb) return;
    if (defaultTargetPlatform != TargetPlatform.android &&
        defaultTargetPlatform != TargetPlatform.iOS) {
      return;
    }

    try {
      await Firebase.initializeApp();

      await _firebaseMessaging.requestPermission(
        alert: true,
        badge: true,
        sound: true,
      );

      const channel = AndroidNotificationChannel(
        'medica_high_importance_channel',
        'Avisos Importantes',
        description: 'Canal usado para lembretes de planejamentos.',
        importance: Importance.high,
      );

      final androidPlugin = _localNotifications
          .resolvePlatformSpecificImplementation<
              AndroidFlutterLocalNotificationsPlugin>();
      await androidPlugin?.createNotificationChannel(channel);

      const initSettings = InitializationSettings(
        android: AndroidInitializationSettings('@mipmap/ic_launcher'),
        iOS: DarwinInitializationSettings(),
      );
      await _localNotifications.initialize(settings: initSettings);

      FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);

      FirebaseMessaging.onMessage.listen((RemoteMessage message) async {
        final notification = message.notification;
        final android = message.notification?.android;

        if (notification != null && android != null) {
          _localNotifications.show(
            id: notification.hashCode,
            title: notification.title,
            body: notification.body,
            notificationDetails: NotificationDetails(
              android: AndroidNotificationDetails(
                channel.id,
                channel.name,
                channelDescription: channel.description,
                icon: '@mipmap/ic_launcher',
                priority: Priority.high,
                importance: Importance.high,
              ),
            ),
          );
        }
      });
    } catch (e) {
      debugPrint('Erro ao inicializar FcmService: $e');
    }
  }

  static Future<String?> getToken() async {
    try {
      if (kIsWeb) {
        debugPrint('FcmService: Plataforma Web detectada (FCM nativo não suportado sem VAPID)');
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
