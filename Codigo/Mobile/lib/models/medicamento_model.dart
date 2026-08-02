/// Representa o medicamento aninhado em cada planejamento retornado pela API.
/// Corresponde ao [MedicamentoMobileDto] do backend.
class MedicamentoModel {
  /// Nome comercial ou genérico do medicamento.
  final String nome;

  /// Apelido opcional definido pelo cuidador (ex: "remédio do coração").
  final String? apelido;

  /// Forma farmacêutica em texto (ex: "COMPRIMIDO", "CAPSULA", "XAROPE").
  final String formaFarmaceutica;

  /// Imagem da caixa do remédio em Base64. Pode estar vazio se não houver foto.
  final String foto;

  MedicamentoModel({
    required this.nome,
    required this.formaFarmaceutica,
    this.apelido,
    this.foto = '',
  });

  factory MedicamentoModel.fromJson(Map<String, dynamic> json) {
    return MedicamentoModel(
      nome: json['nome'] as String? ?? 'Medicamento',
      apelido: json['apelido'] as String?,
      formaFarmaceutica: json['formaFarmaceutica'] as String? ?? '',
      foto: json['foto'] as String? ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'nome': nome,
      'apelido': apelido,
      'formaFarmaceutica': formaFarmaceutica,
      'foto': foto,
    };
  }
}
