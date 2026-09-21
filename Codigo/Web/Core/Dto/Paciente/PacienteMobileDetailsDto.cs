namespace Core.Dto.Paciente
{
    public class PacienteMobileDetailsDto
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Sexo { get; set; } = null!;
        
        public string Escolaridade { get; set; } = null!;

        public string Deficiencia { get; set; } = null!;
    }
}