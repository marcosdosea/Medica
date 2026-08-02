namespace Core.Dto.Planejamento
{
    public class PlanejamentoMobileDto
    {
        public uint Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string DiaSemana { get; set; } = null!;
        public string Horario { get; set; } = null!; // Ex: "08:30"
        public int Dosagem { get; set; }
        public string UnidadeDosagem { get; set; } = null!;

        // Campo novo para o app saber qual ícone central renderizar (ex: "JEJUM", "COM_ALIMENTO")
        public string InstrucaoConsumo { get; set; } = null!;

        public MedicamentoMobileDto Medicamento { get; set; } = null!;
    }

    public class MedicamentoMobileDto
    {
        public string Nome { get; set; } = null!;
        public string? Apelido { get; set; }
        public string FormaFarmaceutica { get; set; } = null!;
        public string Foto { get; set; } = null!; // Base64 da caixa do remédio
    }
}