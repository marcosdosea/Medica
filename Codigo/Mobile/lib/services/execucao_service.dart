import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../config/medica_api_config.dart';
import '../config/session_manager.dart';
import '../models/execucao.dart';

class ExecucaoService {
  final String _baseUrl = MedicaApiConfig.baseUrl;

  Future<bool> registrarExecucao(Execucao execucao) async {
    try {
      final token = await SessionManager.getToken();
      if (token == null || token.isEmpty) return false;

      final uri = Uri.parse('$_baseUrl/Execucao');
      final response = await http
          .post(
            uri,
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer $token',
            },
            body: jsonEncode(execucao.toJson()),
          )
          .timeout(MedicaApiConfig.timeout);

      debugPrint(
        '[ExecucaoService] Status registrarExecucao: ${response.statusCode}',
      );
      return response.statusCode == 200 || response.statusCode == 201;
    } catch (e) {
      debugPrint('[ExecucaoService] Erro ao registrar execução: $e');
      return false;
    }
  }
}
