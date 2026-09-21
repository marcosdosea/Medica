namespace Core.Dto.Planejamento
{
    public class PlanejamentoMobileDto
    {
        public List<PlanejamentoMobileResponseDto> Planejamentos { get; set; } = [];

        public List<int> Excluir { get; set; } = [];

        public DateTime Sincronizacao { get; set; } = DateTime.UtcNow;
    }

    public class PlanejamentoMobileResponseDto
    {
        public int Id { get; set; }

        public uint IdPaciente { get; set; }

        public uint IdMedicamento { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public string DiaSemana { get; set; } = null!;

        public TimeSpan Hora { get; set; }

        public TimeSpan IntervaloExecucao { get; set; }

        public int Dosagem { get; set; }

        public string UnidadeDosagem { get; set; } = null!;
    }

    public class PlanejamentoMobileDetailsDto
    {
        public int Id { get; set; }

        public uint IdPaciente { get; set; }

        public uint IdMedicamento { get; set; }

        public string NomeMedicamento { get; set; } = null!;

        public string? ApelidoMedicamento { get; set; }
        
        public string? FotoMedicamento { get; set; }
    }
}