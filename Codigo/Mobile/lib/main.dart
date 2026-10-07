import 'package:device_preview/device_preview.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'config/api_config.dart';
import 'config/app_colors.dart';
import 'config/http_overrides.dart';
import 'config/session_manager.dart';
import 'services/fcm_service.dart';
import 'views/login_view.dart';
import 'views/placeholder_view.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  if (!kIsWeb &&
      (defaultTargetPlatform == TargetPlatform.android ||
          defaultTargetPlatform == TargetPlatform.iOS)) {
    try {
      await Firebase.initializeApp();
      await FcmService.inicializar();
    } catch (e) {
      debugPrint('Erro ao inicializar Firebase: $e');
    }
  }

  await ApiConfig.inicializarConfiguracoes();

  if (!kIsWeb) {
    MyHttpOverrides.apply();
  }

  final logado = await SessionManager.isLoggedIn();

  runApp(
    DevicePreview(
      enabled: !kReleaseMode,
      builder: (context) => MedicaMobileApp(logado: logado),
    ),
  );
}

class MedicaMobileApp extends StatelessWidget {
  final bool logado;

  const MedicaMobileApp({super.key, required this.logado});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Medica Mobile',
      debugShowCheckedModeBanner: false,
      locale: DevicePreview.locale(context),
      builder: DevicePreview.appBuilder,
      theme: ThemeData(
        useMaterial3: true,
        colorScheme: ColorScheme.fromSeed(
          seedColor: AppColors.primary,
          primary: AppColors.primary,
          secondary: AppColors.secondary,
        ),
        scaffoldBackgroundColor: AppColors.background,
      ),
      home: logado ? const PlaceholderView() : const LoginView(),
    );
  }
}
