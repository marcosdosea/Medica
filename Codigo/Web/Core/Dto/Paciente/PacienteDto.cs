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

        public IEnumerable<ExecucaoDto> ExecucoesFalhas { get; set; } = [];

        public class ExecucaoDto
        {
            public string Data { get; set; } = null!;

            public string Status { get; set; } = null!;

            public string NomeMedicamentoHora { get; set; } = null!;
        }
    }
}