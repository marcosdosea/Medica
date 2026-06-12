namespace Core.Dto.Paciente
{
    public class PacienteDto
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Cpf { get; set; } = null!;

        public DateTime? DataNascimento { get; set; }

        public byte[]? Foto { get; set; }

        public string? Apelido { get; set; }

        public string Ativo { get; set; } = "S";

        public IEnumerable<ExecucaoDto> ExecucoesFalhas { get; set; } = new List<ExecucaoDto>();

        public class ExecucaoDto
        {
            public uint IdPlanejamento { get; set; }

            public int Quantidade { get; set; }

            public string Data { get; set; } = null!;

            public string Status { get; set; } = null!;
        }
    }
}