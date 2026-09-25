import 'dart:convert';
import 'package:flutter/foundation.dart';
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
class AuthService {
  /// Associa este dispositivo ao paciente cujo [tokenPareamento] foi fornecido.
  ///
  /// [tokenPareamento] pode vir de um QR Code (JSON ou String) ou de um PIN digitado pelo usuário.
  static Future<AuthResult> associarDispositivo({
    required String tokenPareamento,
  }) async {
    final rawInput = tokenPareamento.trim();
    if (rawInput.isEmpty) {
      return AuthResult.falha('Informe um token ou PIN válido.');
    }

    // 1. Trata se o valor lido do QR Code veio em formato JSON
    String tokenExtraido = rawInput;
    try {
      final jsonQr = jsonDecode(rawInput);
      if (jsonQr is Map) {
        tokenExtraido = jsonQr['TokenAcesso'] ??
            jsonQr['tokenAcesso'] ??
            jsonQr['tokenPareamento'] ??
            rawInput;
      }
    } catch (_) {
      // O valor enviado é uma String pura (ex: PIN digitado)
    }

    // 📍 LOG DE VERIFICAÇÃO DO TOKEN NO CONSOLE DO FLUTTER
    debugPrint('--------------------------------------------------');
    debugPrint('Token final a ser enviado para a API: $tokenExtraido');
    debugPrint('--------------------------------------------------');

    try {
      // Constrói a URL apontando para a MedicaAPI
      final uri = Uri.parse('${ApiConfig.baseUrl}/Auth/associar-dispositivo');

      // Busca o token do Firebase Cloud Messaging (FCM)
      final fcmToken = await FcmService.getToken();

      final bodyMap = {
        'tokenPareamento': tokenExtraido,
        'fcmToken': fcmToken ?? '',
      };
      final body = jsonEncode(bodyMap);

      final response = await http
          .post(
            uri,
            headers: {'Content-Type': 'application/json'},
            body: body,
          )
          .timeout(ApiConfig.timeout);

      // --- SUCESSO (200) ---
      if (response.statusCode == 200) {
        final jwt = _extrairJwt(response.body);

        if (jwt == null || jwt.isEmpty) {
          return AuthResult.falha('Resposta inválida do servidor.');
        }

        final payload = _decodificarPayload(jwt);
        if (payload == null) {
          return AuthResult.falha('Token inválido recebido do servidor.');
        }

        final idPacienteRaw = payload['IdPaciente'] ?? payload['idPaciente'];
        final idPaciente = idPacienteRaw is int
            ? idPacienteRaw
            : int.tryParse(idPacienteRaw?.toString() ?? '');

        if (idPaciente == null) {
          return AuthResult.falha('Não foi possível identificar o paciente.');
        }

        // Persiste a sessão local via SessionManager
        await SessionManager.saveSession(jwt, 0, idPaciente);
        return AuthResult.ok();
      }

      // --- TRATAMENTO DE ERROS HTTP ---
      if (response.statusCode == 400) {
        debugPrint('--- ERRO 400 DA API ---');
        debugPrint('Payload enviado: $body');
        debugPrint('Resposta da API: ${response.body}');
        debugPrint('------------------------');
        return AuthResult.falha('Dados de pareamento inválidos ou expirados.');
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
          'Sem conexão com a API. Verifique a rede e o adb reverse.',
        );
      }
      return AuthResult.falha('Erro inesperado. Tente novamente.');
    }
  }

  // ── HELPERS PRIVADOS ────────────────────────────────────────────────────────

  /// Extrai o JWT string do corpo da resposta (String pura ou Envelope JSON).
  static String? _extrairJwt(String responseBody) {
    final body = responseBody.trim();

    if (body.startsWith('eyJ')) {
      return body;
    }

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