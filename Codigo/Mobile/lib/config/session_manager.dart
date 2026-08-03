import 'package:shared_preferences/shared_preferences.dart';

/// Gerencia a sessão do usuário logado usando SharedPreferences.
///
/// Armazena o token JWT, ID do grupo e ID da pessoa (paciente)
/// para uso pelos services de comunicação com a API.
class SessionManager {
  static const String _keyToken = 'jwt_token';
  static const String _keyIdGrupo = 'id_grupo';
  static const String _keyIdPessoa = 'id_pessoa';

  /// Salva os dados de sessão após login bem-sucedido.
  static Future<void> saveSession(
    String token,
    int idGrupo,
    int idPessoa,
  ) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_keyToken, token);
    await prefs.setInt(_keyIdGrupo, idGrupo);
    await prefs.setInt(_keyIdPessoa, idPessoa);
  }

  /// Retorna o token JWT da sessão atual, ou null se não houver sessão.
  static Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_keyToken);
  }

  /// Retorna o ID do grupo (cuidador/instituição) da sessão atual.
  static Future<int?> getIdGrupo() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt(_keyIdGrupo);
  }

  /// Retorna o ID da pessoa (paciente) da sessão atual.
  static Future<int?> getIdPessoa() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt(_keyIdPessoa);
  }

  /// Remove todos os dados de sessão (logout).
  /// Também deve ser chamado [HiveService.limparTudo()] para limpar o cache local.
  static Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
  }

  /// Verifica se há uma sessão ativa (token presente).
  static Future<bool> isLoggedIn() async {
    final token = await getToken();
    return token != null && token.isNotEmpty;
  }
}