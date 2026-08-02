namespace Core.Dto.Planejamento
{
    public class PlanejamentoDetailsDto
    {
        public uint Id { get; set; }

        public uint IdPaciente { get; set; }

        public string NomePaciente { get; set; } = null!;

        public string CpfPaciente { get; set; } = null!;

        public uint IdMedicamento { get; set; }

        public string NomeMedicamento { get; set; } = null!;

        public string? ApelidoMedicamento { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public string DiaSemana { get; set; } = null!;

        public TimeSpan Hora { get; set; }

        public int Dosagem { get; set; }

        public Enum.Planejamento.UnidadeDosagem Unidade { get; set; }

        public Core.Enum.Planejamento.Status Status { get; set; }

        public string Ativo { get; set; } = null!;

        public List<ExecucaoDto> Execucoes { get; set; } = new();

        public class ExecucaoDto
        {
            public uint Id { get; set; }

            public string? DataConfirmacao { get; set; }

            public string? HoraConfirmacao { get; set; }

            public Enum.Execucao.Status Status { get; set; }
        }
    }
}