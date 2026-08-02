import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../config/hive_service.dart';
import '../config/session_manager.dart';
import '../models/paciente_model.dart';

/// Serviço responsável por buscar os dados do paciente logado.
///
/// Estratégia de cache (Hive):
/// - Cache de 24h — dados do paciente raramente mudam
/// - Invalidado automaticamente no logout via [SessionManager.clear()]
///
/// Endpoint: GET /paciente/{idPessoa}
class PacienteService {
  static const String _cacheKey = 'paciente_atual';

  /// Retorna os dados do paciente logado.
  ///
  /// [forcarAtualizacao] ignora o cache e vai direto à API.
  Future<PacienteModel?> buscar({bool forcarAtualizacao = false}) async {
    // ── 1. Verificar cache local ───────────────────────────────────────────
    if (!forcarAtualizacao &&
        HiveService.cacheValido(_cacheKey, HiveService.ttlPaciente)) {
      final cached = _lerDoCache();
      if (cached != null) return cached;
    }

    // ── 2. Buscar da API ───────────────────────────────────────────────────
    final idPessoa = await SessionManager.getIdPessoa();
    if (idPessoa == null) return _lerDoCache();

    try {
      final token = await SessionManager.getToken();
      final uri = Uri.parse('${ApiConfig.baseUrl}/paciente/$idPessoa');

      final response = await http.get(
        uri,
        headers: {
          'Content-Type': 'application/json',
          if (token != null) 'Authorization': 'Bearer $token',
        },
      ).timeout(ApiConfig.timeout);

      if (response.statusCode == 200) {
        final body = jsonDecode(response.body) as Map<String, dynamic>;

        // A API retorna: { "sucesso": true, "data": { ... } }
        final dataRaw = body['data'] as Map<String, dynamic>?;
        if (dataRaw == null) return _lerDoCache();

        final paciente = PacienteModel.fromJson(dataRaw);

        // ── 3. Salvar no cache Hive ────────────────────────────────────────
        await _salvarNoCache(paciente);
        await HiveService.marcarCache(_cacheKey);

        return paciente;
      }

      return _lerDoCache();
    } catch (_) {
      // Sem conexão — retorna cache expirado como fallback
      return _lerDoCache();
    }
  }

  /// Invalida o cache do paciente (chamar no logout).
  Future<void> invalidarCache() async {
    await HiveService.invalidarCache(_cacheKey);
    await HiveService.boxPaciente.delete(_cacheKey);
  }

  // ── Helpers de serialização Hive ─────────────────────────────────────────

  PacienteModel? _lerDoCache() {
    final raw = HiveService.boxPaciente.get(_cacheKey);
    if (raw == null) return null;

    try {
      return PacienteModel.fromJson(Map<String, dynamic>.from(raw as Map));
    } catch (_) {
      return null;
    }
  }

  Future<void> _salvarNoCache(PacienteModel paciente) async {
    await HiveService.boxPaciente.put(_cacheKey, paciente.toJson());
  }
}
