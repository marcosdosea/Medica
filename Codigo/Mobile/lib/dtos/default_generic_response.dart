class DefaultGenericResponse<T> {
  final bool sucesso;
  final String mensagem;
  final DateTime timestamp;
  final T? data;

  const DefaultGenericResponse({
    required this.sucesso,
    required this.mensagem,
    required this.timestamp,
    this.data,
  });

  factory DefaultGenericResponse.fromJson(
    Map<String, dynamic> json,
    T Function(dynamic json)? fromJsonT,
  ) {
    final rawData = json['data'];
    return DefaultGenericResponse<T>(
      sucesso: json['sucesso'] as bool,
      mensagem: json['mensagem'] as String,
      timestamp: DateTime.parse(json['timestamp'] as String),
      data: rawData != null && fromJsonT != null
          ? fromJsonT(rawData)
          : rawData as T?,
    );
  }
}
