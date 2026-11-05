class Execucao {
  final int idPlanejamento;
  final double? latitude;
  final double? longitude;
  final String? dataConfirmacao;
  final String? horaConfirmacao;

  const Execucao({
    required this.idPlanejamento,
    this.latitude,
    this.longitude,
    this.dataConfirmacao,
    this.horaConfirmacao,
  });

  factory Execucao.fromJson(Map<String, dynamic> json) {
    return Execucao(
      idPlanejamento: json['idPlanejamento'] as int,
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
      dataConfirmacao: json['dataConfirmacao'] as String?,
      horaConfirmacao: json['horaConfirmacao'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'idPlanejamento': idPlanejamento,
      if (latitude != null) 'latitude': latitude,
      if (longitude != null) 'longitude': longitude,
      if (dataConfirmacao != null) 'dataConfirmacao': dataConfirmacao,
      if (horaConfirmacao != null) 'horaConfirmacao': horaConfirmacao,
    };
  }
}
