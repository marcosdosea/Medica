import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';
import 'fcm_service.dart';

/// Resultado da tentativa de login (associar dispositivo).
class AuthResult {
  /// `true` quando o login foi bem-sucedido.
  final bool sucesso;

  /// Mensagem de erro legível pelo usuário (null quando [sucesso] == true).
  final String? erro;

  const AuthResult._({required this.sucesso, this.erro});

  factory AuthResult.ok() => const AuthResult._(sucesso: true);

  factory AuthResult.falha(String mensagem) =>
      AuthResult._(sucesso: false, erro: mensagem);
}

/// Serviço de autenticação do dispositivo.
///
/// Fluxo:
/// 1. Envia `POST /api/Auth/associar-dispositivo` com o token de pareamento
///    (digitado como PIN ou lido via QR Code) e o FCM Token do aparelho.
/// 2. Recebe o JWT de volta.
/// 3. Decodifica o payload Base64 do JWT para extrair [IdPaciente].
/// 4. Persiste o token e o ID do paciente via [SessionManager].
class AuthService {
  /// Associa este dispositivo ao paciente cujo [tokenPareamento] foi fornecido.
  ///
  /// [tokenPareamento] pode vir de um QR Code ou de um PIN digitado pelo usuário.
  /// [fcmToken] é o token do Firebase Cloud Messaging — pode ser `null` enquanto
  /// o Firebase não estiver configurado; a API aceita string vazia.
  static Future<AuthResult> associarDispositivo({
    required String tokenPareamento,
  }) async {
    if (tokenPareamento.trim().isEmpty) {
      return AuthResult.falha('Informe um token ou PIN válido.');
    }

    try {
      final uri = Uri.parse(
        '${ApiConfig.baseUrl}/api/Auth/associar-dispositivo',
      );

      // Busca o token do Firebase Cloud Messaging para enviar ao servidor
      final fcmToken = await FcmService.getToken();

      final body = jsonEncode({
        'tokenPareamento': tokenPareamento.trim(),
        'fcmToken': fcmToken ?? '',
      });

      final response = await http
          .post(
            uri,
            headers: {'Content-Type': 'application/json'},
            body: body,
          )
          .timeout(ApiConfig.timeout);

      if (response.statusCode == 200) {
        // A API retorna o JWT diretamente como string ou dentro de um envelope.
        final jwt = _extrairJwt(response.body);

        if (jwt == null || jwt.isEmpty) {
          return AuthResult.falha('Resposta inválida do servidor.');
        }

        // Decodifica o payload do JWT para extrair IdPaciente.
        final payload = _decodificarPayload(jwt);
        if (payload == null) {
          return AuthResult.falha('Token inválido recebido do servidor.');
        }

        final idPacienteRaw = payload['IdPaciente'];
        final idPaciente = idPacienteRaw is int
            ? idPacienteRaw
            : int.tryParse(idPacienteRaw?.toString() ?? '');

        if (idPaciente == null) {
          return AuthResult.falha('Não foi possível identificar o paciente.');
        }

        // idGrupo não está no payload — usamos 0 como placeholder.
        await SessionManager.saveSession(jwt, 0, idPaciente);

        return AuthResult.ok();
      }

      if (response.statusCode == 401 || response.statusCode == 403) {
        return AuthResult.falha('Token ou PIN inválido. Verifique e tente novamente.');
      }

      if (response.statusCode == 404) {
        return AuthResult.falha('Nenhum paciente encontrado para este token.');
      }

      return AuthResult.falha(
        'Erro do servidor (${response.statusCode}). Tente novamente.',
      );
    } on Exception catch (e) {
      final msg = e.toString();
      if (msg.contains('TimeoutException') || msg.contains('SocketException')) {
        return AuthResult.falha(
          'Sem conexão com a internet. Verifique sua rede e tente novamente.',
        );
      }
      return AuthResult.falha('Erro inesperado. Tente novamente.');
    }
  }

  // ── Helpers privados ────────────────────────────────────────────────────────

  /// Extrai o JWT string do corpo da resposta, suportando dois formatos:
  /// - String pura: `eyJhbGciOiJ...`
  /// - Envelope JSON: `{ "token": "eyJ...", ... }` ou `{ "data": "eyJ..." }`
  static String? _extrairJwt(String responseBody) {
    final body = responseBody.trim();

    // Formato 1: JWT direto como string
    if (body.startsWith('eyJ')) {
      return body;
    }

    // Formato 2: JSON envelope
    try {
      final json = jsonDecode(body);
      if (json is Map) {
        for (final chave in ['token', 'Token', 'data', 'jwt', 'accessToken']) {
          final valor = json[chave];
          if (valor is String && valor.startsWith('eyJ')) {
            return valor;
          }
        }
      }
      if (json is String && json.startsWith('eyJ')) {
        return json;
      }
    } catch (_) {
      if (body.contains('.')) return body;
    }

    return null;
  }

  /// Decodifica o payload (2ª parte) do JWT sem validar a assinatura.
  static Map<String, dynamic>? _decodificarPayload(String jwt) {
    try {
      final partes = jwt.split('.');
      if (partes.length < 2) return null;

      var base64 = partes[1].replaceAll('-', '+').replaceAll('_', '/');
      while (base64.length % 4 != 0) {
        base64 += '=';
      }

      final decoded = utf8.decode(base64Decode(base64));
      return jsonDecode(decoded) as Map<String, dynamic>;
    } catch (_) {
      return null;
    }
  }
}
