import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/hive_service.dart';
import '../config/session_manager.dart';
import '../models/execucao_model.dart';
import 'planejamento_service.dart';

/// Serviço responsável por registrar a confirmação de tomada de medicamento.
///
/// Fluxo:
/// 1. Envia [ExecucaoModel] via POST /execucao com token JWT
/// 2. Se bem-sucedido → invalida o cache de planejamentos (forçando refresh)
/// 3. Salva registro local no Hive para histórico offline
class ExecucaoService {
  final PlanejamentoService _planejamentoService = PlanejamentoService();

  /// Registra a execução de uma tomada de medicamento.
  ///
  /// Retorna `true` se a API confirmou o registro, `false` caso contrário.
  /// Mesmo em caso de falha de rede, o registro é salvo localmente no Hive.
  Future<bool> registrar(ExecucaoModel execucao) async {
    final token = await SessionManager.getToken();

    bool sucessoApi = false;

    try {
      final response = await http.post(
        Uri.parse('${ApiConfig.baseUrl}/execucao'),
        headers: {
          'Content-Type': 'application/json',
          if (token != null) 'Authorization': 'Bearer $token',
        },
        body: jsonEncode(execucao.toJson()),
      ).timeout(ApiConfig.timeout);

      sucessoApi =
          response.statusCode == 200 || response.statusCode == 201;

      if (sucessoApi) {
        // Invalida o cache de planejamentos para refletir a nova execução
        // na próxima vez que a home page for carregada
        await _planejamentoService.invalidarCache();
      }
    } catch (_) {
      // Falha de rede — o registro local ainda é salvo abaixo
      sucessoApi = false;
    }

    // Sempre salva o registro localmente (histórico offline)
    await _salvarLocal(execucao);

    return sucessoApi;
  }

  /// Retorna o histórico local de execuções armazenadas no Hive.
  /// Útil para exibir histórico mesmo sem conexão.
  Future<List<ExecucaoModel>> historico() async {
    final box = HiveService.boxExecucoes;
    final raw = box.values.toList();

    try {
      return raw
          .whereType<Map>()
          .map((m) => ExecucaoModel.fromJson(Map<String, dynamic>.from(m)))
          .toList()
          .reversed
          .toList(); // Mais recentes primeiro
    } catch (_) {
      return [];
    }
  }

  // ── Helpers privados ───────────────────────────────────────────────────────

  Future<void> _salvarLocal(ExecucaoModel execucao) async {
    final box = HiveService.boxExecucoes;
    // Chave única: idPlanejamento + data + hora para evitar duplicatas
    final chave =
        '${execucao.idPlanejamento}_${execucao.dataConfirmacao}_${execucao.horaConfirmacao ?? ""}';
    await box.put(chave, execucao.toJson());
  }
}