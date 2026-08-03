/// Representa uma execução (confirmação de tomada) de um planejamento.
/// O [toJson] gera o payload enviado ao endpoint POST /execucao,
/// alinhado com [ExecucaoRequestDto] do backend.
class ExecucaoModel {
  /// ID do planejamento que foi executado.
  final int idPlanejamento;

  /// Data da confirmação no formato "yyyy-MM-dd" (ex: "2026-08-01").
  final String dataConfirmacao;

  /// Hora da confirmação no formato "HH:mm:ss" (ex: "08:32:10").
  final String? horaConfirmacao;

  /// Latitude do local da confirmação (opcional, quando GPS disponível).
  final double? latitude;

  /// Longitude do local da confirmação (opcional, quando GPS disponível).
  final double? longitude;

  ExecucaoModel({
    required this.idPlanejamento,
    required this.dataConfirmacao,
    this.horaConfirmacao,
    this.latitude,
    this.longitude,
  });

  /// Cria uma [ExecucaoModel] com data/hora atual.
  factory ExecucaoModel.agora({
    required int idPlanejamento,
    double? latitude,
    double? longitude,
  }) {
    final now = DateTime.now();
    return ExecucaoModel(
      idPlanejamento: idPlanejamento,
      dataConfirmacao:
          '${now.year}-${now.month.toString().padLeft(2, '0')}-${now.day.toString().padLeft(2, '0')}',
      horaConfirmacao:
          '${now.hour.toString().padLeft(2, '0')}:${now.minute.toString().padLeft(2, '0')}:${now.second.toString().padLeft(2, '0')}',
      latitude: latitude,
      longitude: longitude,
    );
  }

  /// Serializa para JSON no formato esperado pelo [ExecucaoRequestDto] da API.
  Map<String, dynamic> toJson() {
    return {
      'idPlanejamento': idPlanejamento,
      'dataConfirmacao': dataConfirmacao,
      'horaConfirmacao': horaConfirmacao,
      'latitude': latitude,
      'longitude': longitude,
    };
  }

  /// Desserializa para uso no histórico local.
  factory ExecucaoModel.fromJson(Map<String, dynamic> json) {
    return ExecucaoModel(
      idPlanejamento: (json['idPlanejamento'] as num?)?.toInt() ?? 0,
      dataConfirmacao: json['dataConfirmacao'] as String? ?? '',
      horaConfirmacao: json['horaConfirmacao'] as String?,
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
    );
  }
}
