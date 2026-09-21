namespace Core.Dto.Planejamento
{
    public class PlanejamentoDto
    {
        public uint Id { get; set; }

        public uint IdPaciente { get; set; }

        public string NomePaciente { get; set; } = null!;

        public uint IdMedicamento { get; set; }

        public string NomeMedicamento { get; set; } = null!;

        public string? ApelidoMedicamento { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public string DiaSemana { get; set; } = null!;

        public TimeSpan Hora { get; set; }

        public int Dosagem { get; set; }

        public string UnidadeDosagem { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Ativo { get; set; } = null!;
    }
}