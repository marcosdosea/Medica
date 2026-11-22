class HomeMock {
  static const String mensagemAguardando =
      'Aguarde o horário do\nmedicamento';
}

class NotificacaoMock {
  static const String tituloSincronizacao = 'Planejamento alterado';
  static const String corpoSincronizacao =
      'Novos medicamentos estão chegando na sua agenda.';
  static const String tituloAlarme = 'Hora do Medicamento!';
  static const String corpoAlarme =
      'Está na hora de tomar o seu medicamento.';
  static const int segundosParaAlarmeTeste = 5;
  static const int duracaoMaximaAlarmeSegundos = 60;
}
