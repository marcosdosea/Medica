import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';

/// Utilitário compartilhado de HTTP para os services do app.
class ApiService {
  /// Monta os headers padrão para requisições à MedicaAPI.
  static Future<Map<String, String>> headers() async {
    final token = await SessionManager.getToken();
    return {
      'Content-Type': 'application/json',
      if (token != null) 'Authorization': 'Bearer $token',
    };
  }

  /// Executa um GET autenticado e retorna o mapa JSON da resposta.
  static Future<Map<String, dynamic>?> get(String endpoint) async {
    final url = '${ApiConfig.baseUrl}$endpoint';
    try {
      final response = await http.get(
        Uri.parse(url),
        headers: await headers(),
      ).timeout(ApiConfig.timeout);

      debugPrint('GET $url -> Status: ${response.statusCode}');

      if (response.statusCode == 200) {
        return jsonDecode(response.body) as Map<String, dynamic>;
      } else {
        debugPrint('--- ERRO NA RESPOSTA GET ($url) ---');
        debugPrint('Status Code: ${response.statusCode}');
        debugPrint('Corpo da Resposta: ${response.body}');
        debugPrint('----------------------------------');
      }
    } catch (e) {
      debugPrint('Exceção ao fazer GET ($url): $e');
    }
    return null;
  }

  /// Executa um POST autenticado com o [body] como JSON.
  static Future<int> post(String endpoint, Map<String, dynamic> body) async {
    final url = '${ApiConfig.baseUrl}$endpoint';
    try {
      final response = await http.post(
        Uri.parse(url),
        headers: await headers(),
        body: jsonEncode(body),
      ).timeout(ApiConfig.timeout);

      debugPrint('POST $url -> Status: ${response.statusCode}');

      // Se for diferente de 200/201 (por exemplo, 400 Bad Request), imprime os detalhes
      if (response.statusCode != 200 && response.statusCode != 201) {
        debugPrint('--- ERRO NA RESPOSTA POST ($url) ---');
        debugPrint('Status Code: ${response.statusCode}');
        debugPrint('Payload Enviado: ${jsonEncode(body)}');
        debugPrint('Corpo do Erro (API): ${response.body}');
        debugPrint('------------------------------------');
      }

      return response.statusCode;
    } catch (e) {
      debugPrint('Exceção ao fazer POST ($url): $e');
      return -1; // Indica falha de rede
    }
  }

  /// Extrai o campo [data] do envelope padrão da API.
  static dynamic extrairData(Map<String, dynamic>? envelope) {
    if (envelope == null) return null;
    if (envelope['sucesso'] != true) return null;
    return envelope['data'];
  }
}