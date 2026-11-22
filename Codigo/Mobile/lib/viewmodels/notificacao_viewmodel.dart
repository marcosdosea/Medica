import 'dart:convert';
import 'package:flutter/foundation.dart';
import '../dtos/planejamento_dto.dart';
import '../models/execucao.dart';
import '../services/execucao_service.dart';
import '../services/fcm_service.dart';
import '../services/planejamento_service.dart';
import '../services/sqflite_service.dart';

class NotificacaoViewModel extends ChangeNotifier {
  final PlanejamentoService _planejamentoService = PlanejamentoService();
  final ExecucaoService _execucaoService = ExecucaoService();
  final SqfliteService _sqfliteService = SqfliteService();

  bool _sincronizando = false;
  bool _alarmeAtivo = false;
  bool _somMutado = false;

  PlanejamentoResponseDto? _planejamentoAtual;
  PlanejamentoDetailsDto? _detalhesAtuais;
  Uint8List? _fotoMedicamentoBytes;

  bool get sincronizando => _sincronizando;
  bool get alarmeAtivo => _alarmeAtivo;
  bool get somMutado => _somMutado;

  String get nomeExibicaoMedicamento {
    final apelido = _detalhesAtuais?.apelidoMedicamento?.trim();
    if (apelido != null && apelido.isNotEmpty) {
      return apelido;
    }
    final nome = _detalhesAtuais?.nomeMedicamento.trim();
    if (nome != null && nome.isNotEmpty) {
      return nome;
    }
    return '';
  }

  String get horarioFormatado {
    final horaRaw = _planejamentoAtual?.hora ?? '';
    if (horaRaw.isEmpty) return '';
    final partes = horaRaw.split(':');
    if (partes.length >= 2) {
      return '${partes[0]}h${partes[1]}';
    }
    return horaRaw;
  }

  int get dosagem => _planejamentoAtual?.dosagem ?? 0;

  Uint8List? get fotoMedicamentoBytes => _fotoMedicamentoBytes;

  String get imagemPeriodoAsset {
    int hora = DateTime.now().hour;
    final horaRaw = _planejamentoAtual?.hora ?? '';
    if (horaRaw.isNotEmpty) {
      final partes = horaRaw.split(':');
      final horaParseada = int.tryParse(partes.first);
      if (horaParseada != null) {
        hora = horaParseada;
      }
    }
    final isNoite = hora < 6 || hora >= 18;
    return isNoite ? 'assets/imgs/h_acordar.png' : 'assets/imgs/h_dormir.png';
  }

  Future<void> inicializar() async {
    FcmService.onAlarmeDisparado = () async {
      await carregarDadosAlarmeAtivo();
      _alarmeAtivo = _planejamentoAtual != null;
      _somMutado = false;
      notifyListeners();
    };

    FcmService.onAlarmeSilenciadoAutomaticamente = () {
      _somMutado = true;
      notifyListeners();
    };

    await FcmService.inicializar();
    await FcmService.verificarAlarmePendenteAoAbrir();
    await _planejamentoService.sincronizarPlanejamentos();
  }

  Future<void> carregarDadosAlarmeAtivo() async {
    var lista = await _sqfliteService.listarPlanejamentos();
    if (lista.isEmpty) {
      lista = await _planejamentoService.sincronizarPlanejamentos();
    }

    if (lista.isEmpty) {
      _planejamentoAtual = null;
      _detalhesAtuais = null;
      _fotoMedicamentoBytes = null;
      return;
    }

    _planejamentoAtual = lista.last;
    _detalhesAtuais = await _planejamentoService.buscarPorId(
      _planejamentoAtual!.id,
    );

    final fotoBase64 = _detalhesAtuais?.fotoMedicamento?.trim();
    if (fotoBase64 != null && fotoBase64.isNotEmpty) {
      try {
        final limpo = fotoBase64.contains(',')
            ? fotoBase64.split(',').last
            : fotoBase64;
        _fotoMedicamentoBytes = base64Decode(limpo);
      } catch (_) {
        _fotoMedicamentoBytes = null;
      }
    } else {
      _fotoMedicamentoBytes = null;
    }
  }

  Future<void> silenciarAlarme() async {
    _somMutado = !_somMutado;
    if (_somMutado) {
      await FcmService.pararSomAlarmeMedicamento();
    } else if (_planejamentoAtual != null) {
      await FcmService.dispararAlarmeMedicamento(
        idPlanejamento: _planejamentoAtual!.id,
      );
    }
    notifyListeners();
  }

  Future<void> confirmarMedicamentoTomado() async {
    await FcmService.pararAlarmeMedicamento();

    final planejamento = _planejamentoAtual;
    _alarmeAtivo = false;
    notifyListeners();

    if (planejamento != null) {
      final agora = DateTime.now();
      final dataStr =
          '${agora.year.toString().padLeft(4, '0')}-${agora.month.toString().padLeft(2, '0')}-${agora.day.toString().padLeft(2, '0')}';
      final horaStr =
          '${agora.hour.toString().padLeft(2, '0')}:${agora.minute.toString().padLeft(2, '0')}:${agora.second.toString().padLeft(2, '0')}';

      await _execucaoService.registrarExecucao(
        Execucao(
          idPlanejamento: planejamento.id,
          dataConfirmacao: dataStr,
          horaConfirmacao: horaStr,
        ),
      );
    }
  }

  Future<void> simularChegadaPlanejamento() async {
    _sincronizando = true;
    notifyListeners();

    await FcmService.processarMensagemSegundoPlano({
      'tipo': FcmService.tipoSincronizarPlanejamentos,
    });

    _sincronizando = false;
    notifyListeners();
  }
}
