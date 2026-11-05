class PlanejamentoDto {
  final List<PlanejamentoResponseDto> planejamentos;
  final List<int> excluir;
  final DateTime sincronizacao;

  const PlanejamentoDto({
    required this.planejamentos,
    required this.excluir,
    required this.sincronizacao,
  });

  factory PlanejamentoDto.fromJson(Map<String, dynamic> json) {
    return PlanejamentoDto(
      planejamentos: (json['planejamentos'] as List<dynamic>)
          .map((e) => PlanejamentoResponseDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      excluir: (json['excluir'] as List<dynamic>)
          .map((e) => e as int)
          .toList(),
      sincronizacao: DateTime.parse(json['sincronizacao'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'planejamentos': planejamentos.map((e) => e.toJson()).toList(),
      'excluir': excluir,
      'sincronizacao': sincronizacao.toIso8601String(),
    };
  }
}

class PlanejamentoResponseDto {
  final int id;
  final int idPaciente;
  final int idMedicamento;
  final DateTime dataInicio;
  final DateTime? dataFim;
  final String diaSemana;
  final String hora;
  final String intervaloExecucao;
  final int dosagem;
  final String unidadeDosagem;

  const PlanejamentoResponseDto({
    required this.id,
    required this.idPaciente,
    required this.idMedicamento,
    required this.dataInicio,
    this.dataFim,
    required this.diaSemana,
    required this.hora,
    required this.intervaloExecucao,
    required this.dosagem,
    required this.unidadeDosagem,
  });

  factory PlanejamentoResponseDto.fromJson(Map<String, dynamic> json) {
    return PlanejamentoResponseDto(
      id: json['id'] as int,
      idPaciente: json['idPaciente'] as int,
      idMedicamento: json['idMedicamento'] as int,
      dataInicio: DateTime.parse(json['dataInicio'] as String),
      dataFim: json['dataFim'] != null
          ? DateTime.parse(json['dataFim'] as String)
          : null,
      diaSemana: json['diaSemana'] as String,
      hora: json['hora'] as String,
      intervaloExecucao: json['intervaloExecucao'] as String,
      dosagem: json['dosagem'] as int,
      unidadeDosagem: json['unidadeDosagem'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'idPaciente': idPaciente,
      'idMedicamento': idMedicamento,
      'dataInicio': dataInicio.toIso8601String(),
      'dataFim': dataFim?.toIso8601String(),
      'diaSemana': diaSemana,
      'hora': hora,
      'intervaloExecucao': intervaloExecucao,
      'dosagem': dosagem,
      'unidadeDosagem': unidadeDosagem,
    };
  }
}

class PlanejamentoDetailsDto {
  final int id;
  final int idPaciente;
  final int idMedicamento;
  final String nomeMedicamento;
  final String? apelidoMedicamento;
  final String? fotoMedicamento;

  const PlanejamentoDetailsDto({
    required this.id,
    required this.idPaciente,
    required this.idMedicamento,
    required this.nomeMedicamento,
    this.apelidoMedicamento,
    this.fotoMedicamento,
  });

  factory PlanejamentoDetailsDto.fromJson(Map<String, dynamic> json) {
    return PlanejamentoDetailsDto(
      id: json['id'] as int,
      idPaciente: json['idPaciente'] as int,
      idMedicamento: json['idMedicamento'] as int,
      nomeMedicamento: json['nomeMedicamento'] as String,
      apelidoMedicamento: json['apelidoMedicamento'] as String?,
      fotoMedicamento: json['fotoMedicamento'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'idPaciente': idPaciente,
      'idMedicamento': idMedicamento,
      'nomeMedicamento': nomeMedicamento,
      'apelidoMedicamento': apelidoMedicamento,
      'fotoMedicamento': fotoMedicamento,
    };
  }
}
