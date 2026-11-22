class SqfliteConfig {
  static const String nomeBanco = 'medica.db';
  static const int versaoBanco = 1;
  static const String tabelaPlanejamentos = 'planejamentos';
  static const String sqlCriarTabelaPlanejamentos = '''
    CREATE TABLE IF NOT EXISTS $tabelaPlanejamentos (
      id INTEGER PRIMARY KEY,
      idPaciente INTEGER NOT NULL,
      idMedicamento INTEGER NOT NULL,
      dataInicio TEXT NOT NULL,
      dataFim TEXT,
      diaSemana TEXT NOT NULL,
      hora TEXT NOT NULL,
      intervaloExecucao TEXT NOT NULL,
      dosagem INTEGER NOT NULL,
      unidadeDosagem TEXT NOT NULL
    )
  ''';
}
