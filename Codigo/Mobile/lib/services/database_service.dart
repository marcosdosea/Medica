import 'package:flutter/foundation.dart';
import '../config/app_mocks.dart';
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

  Future<PlanejamentoResponseDto> sincronizarPlanejamentos() async {
    await inicializar();

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
    debugPrint(
      '[DatabaseService] Planejamento sincronizado localmente para o horário ${planejamentoMock.hora}.',
    );
    return planejamentoMock;
  }
}
