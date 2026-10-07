import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SessionManager {
  static const _secureStorage = FlutterSecureStorage();

  static const String _keyToken = 'jwt_token';
  static const String _keyIdGrupo = 'id_grupo';
  static const String _keyIdPessoa = 'id_pessoa';
  static const String _keyUltimaSincronizacao = 'ultima_sincronizacao';

  static Future<void> saveSession(
    String token,
    int idGrupo,
    int idPessoa,
  ) async {
    await _secureStorage.write(key: _keyToken, value: token);

    final prefs = await SharedPreferences.getInstance();
    await prefs.setInt(_keyIdGrupo, idGrupo);
    await prefs.setInt(_keyIdPessoa, idPessoa);
  }

  static Future<String?> getToken() async {
    return await _secureStorage.read(key: _keyToken);
  }

  static Future<int?> getIdGrupo() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt(_keyIdGrupo);
  }

  static Future<int?> getIdPessoa() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getInt(_keyIdPessoa);
  }

  static Future<void> clear() async {
    await _secureStorage.deleteAll();
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
  }

  static Future<bool> isLoggedIn() async {
    final token = await getToken();
    return token != null && token.isNotEmpty;
  }

  static Future<void> saveUltimaSincronizacao(DateTime data) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_keyUltimaSincronizacao, data.toIso8601String());
  }

  static Future<DateTime?> getUltimaSincronizacao() async {
    final prefs = await SharedPreferences.getInstance();
    final dataStr = prefs.getString(_keyUltimaSincronizacao);
    if (dataStr != null && dataStr.isNotEmpty) {
      return DateTime.tryParse(dataStr);
    }
    return null;
  }
}
