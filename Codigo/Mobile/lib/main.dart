import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'config/api_config.dart';
import 'config/hive_service.dart';
import 'config/http_overrides.dart';
import 'views/home_page.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Inicializa o banco local (Hive) — deve ser o primeiro passo
  await HiveService.inicializar();

  // Configura a URL base da API conforme o ambiente (emulador, aparelho físico, web)
  await ApiConfig.inicializarConfiguracoes();

  // Permite conexões HTTP em desenvolvimento, exceto na web
  if (!kIsWeb) {
    MyHttpOverrides.apply();
  }

  runApp(const MedicaMobileApp());
}

class MedicaMobileApp extends StatelessWidget {
  const MedicaMobileApp({super.key});

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
      home: const HomePage(),
    );
  }
}
