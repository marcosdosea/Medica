import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:device_info_plus/device_info_plus.dart';

class ApiConfig {
  static late String medicApiUrl;
  static String get baseUrl => medicApiUrl;

  static const Duration timeout = Duration(seconds: 10);

  static Future<void> inicializarConfiguracoes() async {
    await dotenv.load(fileName: ".env");
    
    final porta = dotenv.env['API_PORT'] ?? "5066";
    final configuredUrl = dotenv.env['MEDICAPI_URL'];

    if (configuredUrl != null && configuredUrl.isNotEmpty) {
      medicApiUrl = configuredUrl;
      return;
    }

    if (kIsWeb || Platform.isWindows) {
      medicApiUrl = "http://localhost:$porta";
      return;
    }

    if (Platform.isAndroid) {
      DeviceInfoPlugin deviceInfo = DeviceInfoPlugin();
      AndroidDeviceInfo androidInfo = await deviceInfo.androidInfo;

      if (androidInfo.isPhysicalDevice) {
        medicApiUrl = "http://localhost:$porta";
      } else {
        medicApiUrl = "http://10.0.2.2:$porta";
      }
      return;
    }

    medicApiUrl = "http://localhost:$porta";
  }
}