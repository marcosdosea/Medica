namespace Core.Dto.Planejamento
{
    public class PlanejamentoItemDto
    {
        public uint IdPaciente { get; set; }
        public string MedicamentoNome { get; set; } = string.Empty;
        public string DataInicioFormatada { get; set; } = string.Empty;
        public string DataFimFormatada { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string DiaSemana { get; set; } = string.Empty;
        public string Dosagem { get; set; } = string.Empty;
    }
}