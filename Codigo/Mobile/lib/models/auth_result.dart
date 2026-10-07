class AuthResult {
  final bool sucesso;
  final String? erro;

  const AuthResult._({required this.sucesso, this.erro});

  factory AuthResult.ok() => const AuthResult._(sucesso: true);

  factory AuthResult.falha(String mensagem) =>
      AuthResult._(sucesso: false, erro: mensagem);
}
