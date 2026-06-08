using Core.Enum;

namespace Core.Dto.PacienteDto
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
    }
}