class PacienteDetailsDto {
  final int id;
  final String nome;
  final String sexo;
  final String escolaridade;
  final String deficiencia;

  const PacienteDetailsDto({
    required this.id,
    required this.nome,
    required this.sexo,
    required this.escolaridade,
    required this.deficiencia,
  });

  factory PacienteDetailsDto.fromJson(Map<String, dynamic> json) {
    return PacienteDetailsDto(
      id: json['id'] as int,
      nome: json['nome'] as String,
      sexo: json['sexo'] as String,
      escolaridade: json['escolaridade'] as String,
      deficiencia: json['deficiencia'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'nome': nome,
      'sexo': sexo,
      'escolaridade': escolaridade,
      'deficiencia': deficiencia,
    };
  }
}
