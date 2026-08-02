import 'dart:io';

/// Desabilita a verificação de certificado SSL para desenvolvimento.
/// Necessário quando a API usa certificado auto-assinado ou HTTP puro.
///
/// ⚠️ Usar apenas em desenvolvimento. Remover em produção.
class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }

  /// Aplica globalmente o override de HTTP.
  /// Chamar em [main()] antes de [runApp()], apenas em plataformas nativas.
  static void apply() {
    HttpOverrides.global = MyHttpOverrides();
  }
}