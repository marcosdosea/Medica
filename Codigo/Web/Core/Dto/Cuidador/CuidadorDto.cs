namespace Core.Dto.Cuidador
{
    public class CuidadorDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Cpf { get; set; } = null!;

        public string Ativo { get; set; } = "S";

        public int QuantidadePacientes { get; set; }
    }
}
