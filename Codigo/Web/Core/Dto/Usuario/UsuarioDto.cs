namespace Core.Dto.Usuario
{
    public class UsuarioDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Cpf { get; set; } = null!;

        public string Ativo { get; set; } = "S";

        public int QuantidadePacientes { get; set; }

        public string Perfil { get; set; } = "Cuidador";
    }
}
