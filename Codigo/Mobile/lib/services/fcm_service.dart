import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import '../config/session_manager.dart';
import 'planejamento_service.dart';

/// Handler para mensagens em background (quando o app está fechado).
/// Precisa ser uma função de alto nível.
@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp();
  await FcmService._processarNotificacaoSincronizacao(message.data);
}

/// Serviço responsável por inicializar e lidar com Firebase Cloud Messaging (FCM).
class FcmService {
  static final _firebaseMessaging = FirebaseMessaging.instance;
  static final _localNotifications = FlutterLocalNotificationsPlugin();

  /// Inicializa o FCM, solicita permissão (iOS/Android 13+) e configura os listeners.
  static Future<void> inicializar() async {
    // 1. Solicitar permissão para notificações
    await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
    );

    // 2. Configurar canais para Android (para notificação Heads-up em foreground)
    const channel = AndroidNotificationChannel(
      'medica_high_importance_channel', // id
      'Avisos Importantes', // name
      description: 'Canal usado para lembretes de medicação e atualizações.',
      importance: Importance.high,
    );

    final androidPlugin = _localNotifications
        .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>();
    await androidPlugin?.createNotificationChannel(channel);

    // Configuração de ícone para Android (usando o ícone padrão do app)
    const initSettings = InitializationSettings(
      android: AndroidInitializationSettings('@mipmap/ic_launcher'),
      iOS: DarwinInitializationSettings(),
    );
    await _localNotifications.initialize(settings: initSettings);

    // 3. Configurar listeners de recebimento de PUSH
    FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);

    FirebaseMessaging.onMessage.listen((RemoteMessage message) async {
      // Quando o app está aberto, o Firebase não mostra a notificação sozinho.
      // Processamos a sincronização silenciosa e depois mostramos a notificação local.
      await _processarNotificacaoSincronizacao(message.data);

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
  }

  /// Retorna o token do FCM atual (necessário para enviar para a API no login).
  static Future<String?> getToken() async {
    try {
      return await _firebaseMessaging.getToken();
    } catch (_) {
      return null;
    }
  }

  /// Processa a lógica de sincronização a partir do payload recebido pelo FCM.
  /// Se o backend enviar "dataAtualizacao", checamos se é mais nova que a local.
  static Future<void> _processarNotificacaoSincronizacao(
    Map<String, dynamic> data,
  ) async {
    if (!data.containsKey('dataAtualizacao')) return;

    final dataPushStr = data['dataAtualizacao'].toString();
    final dataPush = DateTime.tryParse(dataPushStr);
    
    if (dataPush == null) return;

    final dataLocal = await SessionManager.getUltimaSincronizacao();

    // Se o celular não tem sincronização salva, ou a do Push for mais recente:
    if (dataLocal == null || dataPush.isAfter(dataLocal)) {
      // Baixa os novos dados da API (ignorando cache)
      final planejamentoService = PlanejamentoService();
      await planejamentoService.listar(forcarAtualizacao: true);
      
      // Atualiza a data da última sincronização para a data deste push
      await SessionManager.saveUltimaSincronizacao(dataPush);
    }
  }
}

