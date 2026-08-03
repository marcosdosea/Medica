/// Representa os dados básicos do paciente retornados pela API mobile.
/// Corresponde ao [PacienteMobileDto] do backend.
class PacienteModel {
  /// Indica se o paciente possui alguma deficiência cadastrada.
  final bool possuiDeficiencia;

  /// Sexo do paciente: "M" (Masculino) ou "F" (Feminino).
  final String sexo;

  /// Nível de escolaridade do paciente.
  final String escolaridade;

  /// Lista de deficiências cadastradas para o paciente.
  final List<DeficienciaModel> deficiencias;

  PacienteModel({
    required this.possuiDeficiencia,
    required this.sexo,
    required this.escolaridade,
    this.deficiencias = const [],
  });

  factory PacienteModel.fromJson(Map<String, dynamic> json) {
    final deficienciasRaw = json['deficiencias'] as List<dynamic>? ?? [];
    return PacienteModel(
      possuiDeficiencia: json['possuiDeficiencia'] as bool? ?? false,
      sexo: json['sexo'] as String? ?? '',
      escolaridade: json['escolaridade'] as String? ?? '',
      deficiencias: deficienciasRaw
          .map((d) => DeficienciaModel.fromJson(d as Map<String, dynamic>))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'possuiDeficiencia': possuiDeficiencia,
      'sexo': sexo,
      'escolaridade': escolaridade,
      'deficiencias': deficiencias.map((d) => d.toJson()).toList(),
    };
  }
}

/// Sub-model de deficiência aninhado em [PacienteModel].
/// Corresponde ao [PacienteMobileDto.DeficienciaMobileDto] do backend.
class DeficienciaModel {
  /// Descrição da deficiência (ex: "Visual", "Auditiva").
  final String descricao;

  DeficienciaModel({required this.descricao});

  factory DeficienciaModel.fromJson(Map<String, dynamic> json) {
    return DeficienciaModel(
      descricao: json['descricao'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() => {'descricao': descricao};
}
