import 'package:flutter/foundation.dart';
import '../config/app_mocks.dart';
import '../services/fcm_service.dart';

class NotificacaoViewModel extends ChangeNotifier {
  bool _sincronizando = false;
  bool _alarmeAtivo = false;
  bool _somMutado = false;

  bool get sincronizando => _sincronizando;
  bool get alarmeAtivo => _alarmeAtivo;
  bool get somMutado => _somMutado;

  Future<void> inicializar() async {
    FcmService.onAlarmeDisparado = () {
      _alarmeAtivo = true;
      _somMutado = false;
      notifyListeners();
    };

    FcmService.onAlarmeSilenciadoAutomaticamente = () {
      _somMutado = true;
      notifyListeners();
    };

    await FcmService.inicializar();
  }

  Future<void> silenciarAlarme() async {
    _somMutado = !_somMutado;
    if (_somMutado) {
      await FcmService.pararAlarmeMedicamento();
    } else {
      await FcmService.dispararAlarmeMedicamento();
    }
    notifyListeners();
  }

  Future<void> confirmarMedicamentoTomado() async {
    await FcmService.pararAlarmeMedicamento();
    _alarmeAtivo = false;
    notifyListeners();
  }

  Future<void> simularChegadaPlanejamento() async {
    _sincronizando = true;
    notifyListeners();

    await FcmService.mostrarNotificacaoBanner(
      titulo: NotificacaoMock.tituloSincronizacao,
      corpo: NotificacaoMock.corpoSincronizacao,
      payload: 'SINCRONIZACAO',
    );

    await FcmService.processarMensagemSegundoPlano({
      'tipo': FcmService.tipoSincronizarPlanejamentos,
      'operacao': NotificacaoMock.operacao,
    });

    _sincronizando = false;
    notifyListeners();
  }
}
