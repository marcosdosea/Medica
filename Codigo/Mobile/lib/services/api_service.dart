import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/session_manager.dart';

/// Utilitário compartilhado de HTTP para os services do app.
///
/// Centraliza:
/// - Montagem de headers padrão (Content-Type + Authorization)
/// - Tratamento comum de erros e timeouts
/// - Decodificação do envelope padrão da API: { "sucesso": bool, "data": ... }
///
/// Os services específicos ([PlanejamentoService], [ExecucaoService],
/// [PacienteService]) usam este utilitário internamente.
class ApiService {
  /// Monta os headers padrão para requisições à MedicaAPI.
  /// Inclui o token JWT da sessão se disponível.
  static Future<Map<String, String>> headers() async {
    final token = await SessionManager.getToken();
    return {
      'Content-Type': 'application/json',
      if (token != null) 'Authorization': 'Bearer $token',
    };
  }

  /// Executa um GET autenticado e retorna o mapa JSON da resposta.
  /// Retorna `null` em caso de erro ou status != 200.
  static Future<Map<String, dynamic>?> get(String endpoint) async {
    try {
      final response = await http.get(
        Uri.parse('${ApiConfig.baseUrl}$endpoint'),
        headers: await headers(),
      ).timeout(ApiConfig.timeout);

      if (response.statusCode == 200) {
        return jsonDecode(response.body) as Map<String, dynamic>;
      }
    } catch (_) {
      // Erro de rede ou timeout — retorna null para acionar fallback de cache
    }
    return null;
  }

  /// Executa um POST autenticado com o [body] como JSON.
  /// Retorna o status code da resposta, ou -1 em caso de exceção.
  static Future<int> post(String endpoint, Map<String, dynamic> body) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConfig.baseUrl}$endpoint'),
        headers: await headers(),
        body: jsonEncode(body),
      ).timeout(ApiConfig.timeout);

      return response.statusCode;
    } catch (_) {
      return -1; // Indica falha de rede
    }
  }

  /// Extrai o campo [data] do envelope padrão da API.
  /// Retorna null se o campo não existir ou [sucesso] for false.
  static dynamic extrairData(Map<String, dynamic>? envelope) {
    if (envelope == null) return null;
    if (envelope['sucesso'] != true) return null;
    return envelope['data'];
  }
}
