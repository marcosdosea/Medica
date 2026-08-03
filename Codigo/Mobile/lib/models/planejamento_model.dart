import 'medicamento_model.dart';

/// Representa um planejamento de medicação do paciente.
/// Corresponde ao [PlanejamentoMobileDto] do backend.
///
/// Um paciente pode ter N planejamentos (um por medicamento/horário).
/// A API retorna uma lista via GET /planejamento/mobile/{idPaciente}.
class PlanejamentoModel {
  /// ID único do planejamento no banco de dados.
  final int id;

  /// Data de início do planejamento.
  final DateTime dataInicio;

  /// Data de fim do planejamento.
  final DateTime dataFim;

  /// Dias da semana em que o medicamento deve ser tomado.
  /// Formato: "DSTQQSS" (Dom, Seg, Ter, Qua, Qui, Sex, Sab).
  final String diaSemana;

  /// Horário da tomada no formato "HH:mm" (ex: "08:30").
  final String horario;

  /// Intervalo entre execuções no formato "HH:mm:ss" (ex: "24:00:00").
  final String intervaloExecucao;

  /// Quantidade de unidades a serem tomadas por vez.
  final int dosagem;

  /// Unidade de dosagem (ex: "G", "UI", "ML", "COMPRIMIDO").
  final String unidadeDosagem;

  /// Instrução de consumo em relação às refeições.
  /// Ex: "JEJUM", "COM_ALIMENTO", "APOS_REFEICAO".
  final String instrucaoConsumo;

  /// Dados do medicamento associado a este planejamento.
  final MedicamentoModel medicamento;

  PlanejamentoModel({
    required this.id,
    required this.dataInicio,
    required this.dataFim,
    required this.diaSemana,
    required this.horario,
    required this.intervaloExecucao,
    required this.dosagem,
    required this.unidadeDosagem,
    required this.instrucaoConsumo,
    required this.medicamento,
  });

  /// Cria um [PlanejamentoModel] a partir do JSON retornado pela API.
  factory PlanejamentoModel.fromJson(Map<String, dynamic> json) {
    return PlanejamentoModel(
      id: (json['id'] as num?)?.toInt() ?? 0,
      dataInicio: _parseDate(json['dataInicio']),
      dataFim: _parseDate(json['dataFim']),
      diaSemana: json['diaSemana'] as String? ?? '',
      horario: json['horario'] as String? ?? '00:00',
      intervaloExecucao: json['intervaloExecucao'] as String? ?? '24:00:00',
      dosagem: (json['dosagem'] as num?)?.toInt() ?? 1,
      unidadeDosagem: json['unidadeDosagem'] as String? ?? '',
      instrucaoConsumo: json['instrucaoConsumo'] as String? ?? '',
      medicamento: MedicamentoModel.fromJson(
        json['medicamento'] as Map<String, dynamic>? ?? {},
      ),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'dataInicio': dataInicio.toIso8601String(),
      'dataFim': dataFim.toIso8601String(),
      'diaSemana': diaSemana,
      'horario': horario,
      'intervaloExecucao': intervaloExecucao,
      'dosagem': dosagem,
      'unidadeDosagem': unidadeDosagem,
      'instrucaoConsumo': instrucaoConsumo,
      'medicamento': medicamento.toJson(),
    };
  }

  /// Verifica se o planejamento está ativo hoje (data atual entre início e fim).
  bool get isAtivo {
    final hoje = DateTime.now();
    return !hoje.isBefore(dataInicio) && !hoje.isAfter(dataFim);
  }

  /// Retorna true se o dia de hoje está incluído no [diaSemana].
  bool get isHoje {
    const dias = ['DOM', 'SEG', 'TER', 'QUA', 'QUI', 'SEX', 'SAB'];
    final diaIndex = DateTime.now().weekday % 7; // 0=Dom, 1=Seg, ...6=Sab
    return diaSemana.contains(dias[diaIndex]);
  }

  static DateTime _parseDate(dynamic value) {
    if (value == null) return DateTime.now();
    if (value is String) return DateTime.tryParse(value) ?? DateTime.now();
    return DateTime.now();
  }
}
