import 'package:flutter/foundation.dart';
import 'package:path/path.dart' as p;
import 'package:sqflite/sqflite.dart';
import '../config/sqflite_config.dart';
import '../dtos/planejamento_dto.dart';

class SqfliteService {
  static final SqfliteService _instance = SqfliteService._internal();

  factory SqfliteService() => _instance;

  SqfliteService._internal();

  Database? _db;

  Future<Database> get database async {
    if (_db != null) return _db!;
    await inicializar();
    return _db!;
  }

  Future<void> inicializar() async {
    if (_db != null) return;

    final dbPath = await getDatabasesPath();
    final path = p.join(dbPath, SqfliteConfig.nomeBanco);

    _db = await openDatabase(
      path,
      version: SqfliteConfig.versaoBanco,
      onCreate: (db, version) async {
        await db.execute(SqfliteConfig.sqlCriarTabelaPlanejamentos);
      },
    );

    await _db!.execute(SqfliteConfig.sqlCriarTabelaPlanejamentos);
    debugPrint('[SqfliteService] SQLite inicializado em: $path');
  }

  Future<void> substituirPlanejamentos(
    List<PlanejamentoResponseDto> planejamentos,
  ) async {
    final db = await database;
    final batch = db.batch();

    batch.delete(SqfliteConfig.tabelaPlanejamentos);

    for (final item in planejamentos) {
      batch.insert(
        SqfliteConfig.tabelaPlanejamentos,
        item.toJson(),
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }

    await batch.commit(noResult: true);
    debugPrint(
      '[SqfliteService] ${planejamentos.length} planejamento(s) sincronizado(s) no SQLite.',
    );
  }

  Future<void> salvarPlanejamentos(
    List<PlanejamentoResponseDto> planejamentos,
  ) async {
    final db = await database;
    final batch = db.batch();

    for (final item in planejamentos) {
      batch.insert(
        SqfliteConfig.tabelaPlanejamentos,
        item.toJson(),
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }

    await batch.commit(noResult: true);
    debugPrint(
      '[SqfliteService] ${planejamentos.length} planejamento(s) salvo(s)/atualizado(s) no SQLite.',
    );
  }

  Future<void> excluirPlanejamentos(List<int> ids) async {
    if (ids.isEmpty) return;

    final db = await database;
    final placeholders = List.filled(ids.length, '?').join(',');
    await db.delete(
      SqfliteConfig.tabelaPlanejamentos,
      where: 'id IN ($placeholders)',
      whereArgs: ids,
    );

    debugPrint(
      '[SqfliteService] ${ids.length} planejamento(s) excluído(s) do SQLite.',
    );
  }

  Future<List<PlanejamentoResponseDto>> listarPlanejamentos() async {
    final db = await database;
    final rows = await db.query(SqfliteConfig.tabelaPlanejamentos);
    return rows.map((row) => PlanejamentoResponseDto.fromJson(row)).toList();
  }
}
