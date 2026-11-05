class HomeMock {
  static const String nomePaciente = 'Meta';
  static const String horario = '12h30';
  static const int dosagem = 1;
  static const String imagemMedicamento = 'assets/imgs/logo_splash.png';
  static const String mensagemAguardando =
      'Aguarde o horário do\nmedicamento';
}

class NotificacaoMock {
  static const String tituloSincronizacao = 'Planejamento alterado';
  static const String corpoSincronizacao =
      'Novos medicamentos estão chegando na sua agenda.';
  static const String tituloAlarme = 'Hora do Medicamento!';
  static const String corpoAlarme = 'Meta: tomar 1 comprimido às 12h30.';
  static const String payload = '1';
  static const String operacao = 'CRIAR_OU_EDITAR';
  static const int segundosParaAlarmeTeste = 5;
  static const int duracaoMaximaAlarmeSegundos = 60;
}
