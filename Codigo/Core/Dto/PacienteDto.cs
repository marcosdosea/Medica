namespace Core.Dto
{
    public class PacienteDto
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string? Apelido { get; set; }

        public byte[]? Foto { get; set; }
    }
}
