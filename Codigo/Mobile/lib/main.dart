import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:firebase_core/firebase_core.dart';
import 'config/api_config.dart';
import 'config/hive_service.dart';
import 'config/http_overrides.dart';
import 'config/session_manager.dart';
import 'services/fcm_service.dart';
import 'views/home_page.dart';
import 'views/login_view.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Inicializa o banco local (Hive) — deve ser o primeiro passo
  await HiveService.inicializar();

  // Inicializa Firebase (necessita do google-services.json que será adicionado manualmente)
  try {
    await Firebase.initializeApp();
    await FcmService.inicializar();
  } catch (e) {
    debugPrint('Firebase ainda não configurado (faltando google-services.json): $e');
  }

  // Configura a URL base da API conforme o ambiente (emulador, aparelho físico, web)

  await ApiConfig.inicializarConfiguracoes();

  // Permite conexões HTTP em desenvolvimento, exceto na web
  if (!kIsWeb) {
    MyHttpOverrides.apply();
  }

  // Verifica se já existe uma sessão JWT ativa
  final logado = await SessionManager.isLoggedIn();

  runApp(MedicaMobileApp(logado: logado));
}

class MedicaMobileApp extends StatelessWidget {
  final bool logado;

  const MedicaMobileApp({super.key, required this.logado});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Medica Mobile',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        useMaterial3: true,
        colorScheme: ColorScheme.fromSeed(
          seedColor: const Color(0xFF2563EB),
          primary: const Color(0xFF2563EB),
          secondary: const Color(0xFF22C55E),
        ),
        scaffoldBackgroundColor: const Color(0xFFF1F5F9),
      ),
      // Rota inicial baseada na sessão
      home: logado ? const HomePage() : const LoginView(),
    );
  }
}

