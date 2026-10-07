import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';
import '../models/auth_result.dart';
import 'fcm_service.dart';

class AuthService {
  static Future<AuthResult> associarDispositivo({
    required String tokenPareamento,
  }) async {
    final rawInput = tokenPareamento.trim();
    if (rawInput.isEmpty) {
      return AuthResult.falha('Informe um token ou PIN válido.');
    }

    String tokenExtraido = rawInput;
    try {
      final jsonQr = jsonDecode(rawInput);
      if (jsonQr is Map) {
        tokenExtraido = jsonQr['TokenAcesso'] ??
            jsonQr['tokenAcesso'] ??
            jsonQr['tokenPareamento'] ??
            jsonQr['token'] ??
            rawInput;
      }
    } catch (_) {}

    debugPrint('Token para login extraído');

    try {
      final baseUrl = ApiConfig.medicApiUrl.endsWith('/api')
          ? ApiConfig.medicApiUrl
          : '${ApiConfig.medicApiUrl}/api';
      final uri = Uri.parse('$baseUrl/Auth/associar-dispositivo');

      final fcmToken = await FcmService.getToken();
      final bool fcmValido = fcmToken != null && fcmToken.isNotEmpty;
      final debugToken = dotenv.env['DEBUG_FCM_TOKEN'] ?? 'device_fcm_token_placeholder';
      final fcmFinal = fcmValido ? fcmToken : debugToken;

      if (fcmValido) {
        debugPrint('[AuthService] Enviando Token FCM REAL do dispositivo.');
      } else {
        debugPrint('[AuthService] AVISO: FCM Token nulo/indisponível. Enviando fallback do .env (DEBUG_FCM_TOKEN): $fcmFinal');
      }

      final body = jsonEncode({
        'tokenPareamento': tokenExtraido,
        'fcmToken': fcmFinal,
      });

      final response = await http
          .post(
            uri,
            headers: {'Content-Type': 'application/json'},
            body: body,
          )
          .timeout(ApiConfig.timeout);

      debugPrint('Status da resposta: ${response.statusCode}');
      debugPrint('Corpo da resposta: ${response.body}');

      if (response.statusCode == 200) {
        final jwt = _extrairJwt(response.body);

        if (jwt == null || jwt.isEmpty) {
          return AuthResult.falha('Resposta inválida do servidor (JWT não encontrado).');
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
          return AuthResult.falha('Não foi possível identificar o paciente no token.');
        }

        await SessionManager.saveSession(jwt, 0, idPaciente);
        return AuthResult.ok();
      }

      if (response.statusCode == 400) {
        try {
          final erroJson = jsonDecode(response.body);
          if (erroJson is Map && erroJson['mensagem'] != null) {
            return AuthResult.falha(erroJson['mensagem']);
          }
        } catch (_) {}
        return AuthResult.falha('Dados de pareamento inválidos ou expirados.');
      }

      if (response.statusCode == 401 || response.statusCode == 403) {
        return AuthResult.falha('Token inválido ou não autorizado.');
      }

      if (response.statusCode == 404) {
        return AuthResult.falha('Serviço não encontrado. Verifique a URL da API');
      }

      return AuthResult.falha(
        'Erro do servidor (${response.statusCode}): ${response.body}',
      );
    } on Exception catch (e) {
      debugPrint('Erro em associarDispositivo: $e');
      final msg = e.toString();
      if (msg.contains('TimeoutException')) {
        return AuthResult.falha('Tempo limite esgotado ao conectar com a API');
      }
      if (msg.contains('SocketException') || msg.contains('Connection refused') || msg.contains('Failed host lookup')) {
        return AuthResult.falha('Sem conexão com a API');
      }
      return AuthResult.falha('Falha na conexão');
    }
  }

  static String? _extrairJwt(String responseBody) {
    final body = responseBody.trim();

    if (body.startsWith('eyJ')) {
      return body;
    }

    try {
      final json = jsonDecode(body);
      if (json is Map) {
        final data = json['data'];
        if (data is Map) {
          final token = data['tokenJwt'];
          if (token is String && token.isNotEmpty) {
            return token;
          }
        } else if (data is String && data.startsWith('eyJ')) {
          return data;
        }

        final valor = json['tokenJwt'];
        if (valor is String && valor.isNotEmpty) {
          return valor;
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