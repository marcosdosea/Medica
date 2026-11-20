import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/app_mocks.dart';
import '../config/session_manager.dart';
import '../dtos/default_generic_response.dart';
import '../dtos/planejamento_dto.dart';

class DatabaseService {
  static final DatabaseService _instance = DatabaseService._internal();

  factory DatabaseService() => _instance;

  DatabaseService._internal();

  static const String nomeBanco = 'medica.db';
  static const int versaoBanco = 1;
  static const String tabelaPlanejamentos = 'planejamentos';
  static const String sqlCriarTabelaPlanejamentos = '''
    CREATE TABLE IF NOT EXISTS $tabelaPlanejamentos (
      id INTEGER PRIMARY KEY,
      idPaciente INTEGER NOT NULL,
      idMedicamento INTEGER NOT NULL,
      dataInicio TEXT NOT NULL,
      dataFim TEXT,
      diaSemana TEXT NOT NULL,
      hora TEXT NOT NULL,
      intervaloExecucao TEXT NOT NULL,
      dosagem INTEGER NOT NULL,
      unidadeDosagem TEXT NOT NULL
    )
  ''';

  final Map<int, PlanejamentoResponseDto> _cacheLocal = {};

  Future<void> inicializar() async {
    debugPrint(
      '[DatabaseService] Estrutura SQLite preparada ($nomeBanco v$versaoBanco).',
    );
  }

  Future<void> salvarPlanejamentos(
    List<PlanejamentoResponseDto> planejamentos,
  ) async {
    for (final item in planejamentos) {
      _cacheLocal[item.id] = item;
    }
    debugPrint(
      '[DatabaseService] ${planejamentos.length} planejamento(s) salvo(s)/atualizado(s) localmente.',
    );
  }

  Future<void> excluirPlanejamentos(List<int> ids) async {
    for (final id in ids) {
      _cacheLocal.remove(id);
    }
    debugPrint(
      '[DatabaseService] ${ids.length} planejamento(s) excluído(s) localmente.',
    );
  }

  Future<List<PlanejamentoResponseDto>> listarPlanejamentos() async {
    return _cacheLocal.values.toList();
  }

  Future<List<PlanejamentoResponseDto>> sincronizarPlanejamentos() async {
    await inicializar();

    try {
      await ApiConfig.inicializarConfiguracoes();
      final token = await SessionManager.getToken();
      final idPaciente = await SessionManager.getIdPessoa();
      final ultimaSincronizacao = await SessionManager.getUltimaSincronizacao();

      if (token != null && token.isNotEmpty && idPaciente != null) {
        final baseUrl = ApiConfig.baseUrl.endsWith('/api')
            ? ApiConfig.baseUrl.substring(0, ApiConfig.baseUrl.length - 4)
            : ApiConfig.baseUrl;

        final queryParams = <String, String>{};
        if (ultimaSincronizacao != null) {
          queryParams['ultimaSincronizacao'] =
              ultimaSincronizacao.toUtc().toIso8601String();
        }

        final uri = Uri.parse('$baseUrl/Planejamento/paciente/$idPaciente')
            .replace(queryParameters: queryParams.isEmpty ? null : queryParams);

        debugPrint('[DatabaseService] Sincronizando com API: $uri');

        final response = await http
            .get(
              uri,
              headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer $token',
              },
            )
            .timeout(ApiConfig.timeout);

        debugPrint(
          '[DatabaseService] Status sincronização: ${response.statusCode}',
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
              await excluirPlanejamentos(dto.excluir);
            }
            if (dto.planejamentos.isNotEmpty) {
              await salvarPlanejamentos(dto.planejamentos);
            }
            await SessionManager.saveUltimaSincronizacao(dto.sincronizacao);
            return listarPlanejamentos();
          }
        }
      }
    } catch (e) {
      debugPrint('[DatabaseService] Erro ao sincronizar com API: $e');
    }

    final planejamentoMock = PlanejamentoResponseDto(
      id: 1,
      idPaciente: 11,
      idMedicamento: 1,
      dataInicio: DateTime.now(),
      diaSemana: 'SEG,TER,QUA,QUI,SEX,SAB,DOM',
      hora: HomeMock.horario,
      intervaloExecucao: '08:00:00',
      dosagem: HomeMock.dosagem,
      unidadeDosagem: 'COMPRIMIDO',
    );

    await salvarPlanejamentos([planejamentoMock]);
    return listarPlanejamentos();
  }
}
