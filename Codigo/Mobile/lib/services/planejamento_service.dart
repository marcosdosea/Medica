import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/hive_service.dart';
import '../config/session_manager.dart';
import '../models/planejamento_model.dart';

/// Serviço responsável por buscar os planejamentos de medicação do paciente.
///
/// Estratégia de cache (Hive):
/// 1. Se houver dados em cache válidos (TTL: 1h) → retorna do Hive
/// 2. Se não → busca da API [GET /planejamento/mobile/{idPaciente}]
/// 3. Salva o resultado no Hive e retorna
///
/// O cache é invalidado após cada execução bem-sucedida (ver [ExecucaoService]).
class PlanejamentoService {
  static const String _cacheKey = 'planejamentos';

  /// Retorna a lista de planejamentos ativos do paciente logado.
  ///
  /// [forcarAtualizacao] ignora o cache e vai direto à API.
  Future<List<PlanejamentoModel>> listar({bool forcarAtualizacao = false}) async {
    // ── 1. Verificar cache local ───────────────────────────────────────────
    if (!forcarAtualizacao &&
        HiveService.cacheValido(_cacheKey, HiveService.ttlPlanejamentos)) {
      final cached = _lerDoCache();
      if (cached != null && cached.isNotEmpty) {
        return cached;
      }
    }

    // ── 2. Buscar da API ───────────────────────────────────────────────────
    final idPaciente = await SessionManager.getIdPessoa();
    if (idPaciente == null) {
      // Sem sessão ativa — retorna cache expirado se existir, ou lista vazia
      return _lerDoCache() ?? [];
    }

    try {
      final token = await SessionManager.getToken();
      final uri = Uri.parse('${ApiConfig.baseUrl}/planejamento/mobile/$idPaciente');

      final response = await http.get(
        uri,
        headers: {
          'Content-Type': 'application/json',
          if (token != null) 'Authorization': 'Bearer $token',
        },
      ).timeout(ApiConfig.timeout);

      if (response.statusCode == 200) {
        final body = jsonDecode(response.body) as Map<String, dynamic>;

        // A API retorna: { "sucesso": true, "data": [...] }
        final dataRaw = body['data'];
        if (dataRaw is! List) return _lerDoCache() ?? [];

        final planejamentos = dataRaw
            .map((item) =>
                PlanejamentoModel.fromJson(item as Map<String, dynamic>))
            .toList();

        // ── 3. Salvar no cache Hive ────────────────────────────────────────
        await _salvarNoCache(planejamentos);
        await HiveService.marcarCache(_cacheKey);

        return planejamentos;
      }

      // API retornou erro — usa cache expirado como fallback
      return _lerDoCache() ?? [];
    } catch (_) {
      // Sem conexão — usa cache expirado como fallback
      return _lerDoCache() ?? [];
    }
  }

  /// Busca um único planejamento por [id] a partir da lista em cache.
  /// Evita chamada de rede se os dados já estiverem localmente disponíveis.
  Future<PlanejamentoModel?> buscarPorId(int id, {bool forcarAtualizacao = false}) async {
    final lista = await listar(forcarAtualizacao: forcarAtualizacao);
    try {
      return lista.firstWhere((p) => p.id == id);
    } catch (_) {
      return null;
    }
  }

  /// Invalida o cache de planejamentos (chamado pelo [ExecucaoService] após
  /// uma execução bem-sucedida, para refletir a nova execução na próxima leitura).
  Future<void> invalidarCache() async {
    await HiveService.invalidarCache(_cacheKey);
  }

  // ── Helpers de serialização Hive ─────────────────────────────────────────

  List<PlanejamentoModel>? _lerDoCache() {
    final box = HiveService.boxPlanejamentos;
    final raw = box.get(_cacheKey);
    if (raw == null) return null;

    try {
      final lista = (raw as List).cast<Map>();
      return lista
          .map((m) => PlanejamentoModel.fromJson(Map<String, dynamic>.from(m)))
          .toList();
    } catch (_) {
      return null;
    }
  }

  Future<void> _salvarNoCache(List<PlanejamentoModel> planejamentos) async {
    final box = HiveService.boxPlanejamentos;
    final encoded = planejamentos.map((p) => p.toJson()).toList();
    await box.put(_cacheKey, encoded);
  }
}
