namespace Core.Dto.Planejamento
{
    public class PlanejamentoItemDto
    {
        public int Id { get; set; }
        public uint IdPaciente { get; set; }
        public uint IdMedicamento { get; set; }
        public string MedicamentoNome { get; set; } = string.Empty;
        public string DataInicioFormatada { get; set; } = string.Empty;
        public string DataInicioIso { get; set; } = string.Empty;
        public string DataFimFormatada { get; set; } = string.Empty;
        public string DataFimIso { get; set; } = string.Empty;
        public bool Continuo { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string IntervaloFormatado { get; set; } = string.Empty;
        public string DiaSemana { get; set; } = string.Empty;
        public string Dosagem { get; set; } = string.Empty;
        public int DosagemValor { get; set; }
        public string UnidadeDosagem { get; set; } = string.Empty;
    }
}