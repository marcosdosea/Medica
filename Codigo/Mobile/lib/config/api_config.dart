import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:device_info_plus/device_info_plus.dart';

class ApiConfig {

  static late String baseUrl;
  static late String webBaseUrl;
  
  static const Duration timeout = Duration(seconds: 10);

  static Future<void> inicializarConfiguracoes() async {
    // Porta HTTP da sua API .NET (C:\dev\medica\Codigo\API)
    const String apiPorta = "5066";
    
    if (kIsWeb) {
      baseUrl = "http://localhost:$apiPorta";
      return;
    }

    if (Platform.isWindows) {
      baseUrl = "http://localhost:$apiPorta";
      webBaseUrl = "https://batala.itatechjr.com.br";
      return;
    }

    if (Platform.isAndroid) {
      DeviceInfoPlugin deviceInfo = DeviceInfoPlugin();
      AndroidDeviceInfo androidInfo = await deviceInfo.androidInfo;

      if (androidInfo.isPhysicalDevice) {
        // Se estiver depurando via CABO USB com 'adb reverse tcp:5066 tcp:5066', use localhost.
        // Se estiver depurando via Wi-Fi sem ADB, use o IP da sua máquina: "http://192.168.0.109:$apiPorta"
        baseUrl = "http://localhost:$apiPorta";
        webBaseUrl = "http://192.168.0.109:5051";
      } else {
        // Se for o Emulador do Android Studio
        baseUrl = "http://10.0.2.2:$apiPorta";
        webBaseUrl = "http://10.0.2.2:5051";
      }
      return;
    }
    
    // Fallback padrão de segurança
    baseUrl = "http://localhost:$apiPorta";
    webBaseUrl = "https://localhost:7112";
  }
}