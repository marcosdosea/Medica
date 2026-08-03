import 'package:flutter/foundation.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:path_provider/path_provider.dart';

class HiveService {
  static const String _boxPlanejamentos = 'planejamentos';
  static const String _boxExecucoes = 'execucoes';
  static const String _boxPaciente = 'paciente';
  static const String _boxMeta = 'meta';

  static const Duration ttlPlanejamentos = Duration(hours: 1);
  static const Duration ttlPaciente = Duration(hours: 24);

  static Future<void> inicializar() async {
    if (kIsWeb) {
      // No navegador, o Hive usa IndexedDB.
      await Hive.initFlutter();
    } else {
      // No Windows, Android e demais plataformas nativas.
      final diretorio = await getApplicationSupportDirectory();

      if (!await diretorio.exists()) {
        await diretorio.create(recursive: true);
      }

      await Hive.initFlutter(diretorio.path);

      debugPrint('Diretório do Hive: ${diretorio.path}');
    }

    await Future.wait([
      Hive.openBox<dynamic>(_boxPlanejamentos),
      Hive.openBox<dynamic>(_boxExecucoes),
      Hive.openBox<dynamic>(_boxPaciente),
      Hive.openBox<dynamic>(_boxMeta),
    ]);
  }

  static Box<dynamic> get boxPlanejamentos =>
      Hive.box<dynamic>(_boxPlanejamentos);

  static Box<dynamic> get boxExecucoes =>
      Hive.box<dynamic>(_boxExecucoes);

  static Box<dynamic> get boxPaciente =>
      Hive.box<dynamic>(_boxPaciente);

  static Box<dynamic> get boxMeta =>
      Hive.box<dynamic>(_boxMeta);

  static bool cacheValido(String chave, Duration ttl) {
    final ms = boxMeta.get('ts_$chave') as int?;

    if (ms == null) {
      return false;
    }

    final cachedAt = DateTime.fromMillisecondsSinceEpoch(ms);

    return DateTime.now().difference(cachedAt) < ttl;
  }

  static Future<void> marcarCache(String chave) async {
    await boxMeta.put(
      'ts_$chave',
      DateTime.now().millisecondsSinceEpoch,
    );
  }

  static Future<void> invalidarCache(String chave) async {
    await boxMeta.delete('ts_$chave');
  }

  static Future<void> limparTudo() async {
    await Future.wait([
      boxPlanejamentos.clear(),
      boxExecucoes.clear(),
      boxPaciente.clear(),
      boxMeta.clear(),
    ]);
  }
}