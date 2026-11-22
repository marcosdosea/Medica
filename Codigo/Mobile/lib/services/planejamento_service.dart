import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../config/medica_api_config.dart';
import '../config/session_manager.dart';
import '../dtos/default_generic_response.dart';
import '../dtos/planejamento_dto.dart';
import 'sqflite_service.dart';

class PlanejamentoService {
  final SqfliteService _sqfliteService = SqfliteService();
  final String _baseUrl = MedicaApiConfig.baseUrl;

  Future<List<PlanejamentoResponseDto>> sincronizarPlanejamentos() async {
    try {
      final token = await SessionManager.getToken();
      final idPaciente = await SessionManager.getIdPessoa();
      final ultimaSincronizacao = await SessionManager.getUltimaSincronizacao();

      if (token != null && token.isNotEmpty && idPaciente != null) {
        final queryParams = <String, String>{};
        if (ultimaSincronizacao != null) {
          queryParams['ultimaSincronizacao'] =
              ultimaSincronizacao.toUtc().toIso8601String();
        }

        final uri = Uri.parse('$_baseUrl/Planejamento/paciente/$idPaciente')
            .replace(queryParameters: queryParams.isEmpty ? null : queryParams);

        debugPrint('[PlanejamentoService] Sincronizando com API: $uri');

        final response = await http
            .get(
              uri,
              headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer $token',
              },
            )
            .timeout(MedicaApiConfig.timeout);

        debugPrint(
          '[PlanejamentoService] Status sincronização: ${response.statusCode}',
        );

        if (response.statusCode == 200) {
          final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
          final apiResponse = DefaultGenericResponse<PlanejamentoDto>.fromJson(
            jsonMap,
            (data) => PlanejamentoDto.fromJson(data as Map<String, dynamic>),
          );

          if (apiResponse.sucesso && apiResponse.data != null) {
            final dto = apiResponse.data!;
            if (dto.excluir.isNotEmpty) {
              await _sqfliteService.excluirPlanejamentos(dto.excluir);
            }
            if (dto.planejamentos.isNotEmpty) {
              await _sqfliteService.salvarPlanejamentos(dto.planejamentos);
            }
            await SessionManager.saveUltimaSincronizacao(dto.sincronizacao);
          }
        }
      }
    } catch (e) {
      debugPrint('[PlanejamentoService] Erro ao sincronizar com API: $e');
    }

    return _sqfliteService.listarPlanejamentos();
  }

  Future<PlanejamentoDetailsDto?> buscarPorId(int id) async {
    try {
      final token = await SessionManager.getToken();
      if (token == null || token.isEmpty) return null;

      final uri = Uri.parse('$_baseUrl/Planejamento/$id');

      final response = await http
          .get(
            uri,
            headers: {
              'Content-Type': 'application/json',
              'Authorization': 'Bearer $token',
            },
          )
          .timeout(MedicaApiConfig.timeout);

      if (response.statusCode == 200) {
        final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
        final apiResponse =
            DefaultGenericResponse<PlanejamentoDetailsDto>.fromJson(
          jsonMap,
          (data) =>
              PlanejamentoDetailsDto.fromJson(data as Map<String, dynamic>),
        );

        if (apiResponse.sucesso) {
          return apiResponse.data;
        }
      }
    } catch (e) {
      debugPrint('[PlanejamentoService] Erro ao buscar planejamento $id: $e');
    }

    return null;
  }
}
