namespace Core.Dto.PacienteDto
{
    public class PacienteDto
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Cpf { get; set; } = null!;

        public string Sexo { get; set; } = null!;

        public DateTime? DataNascimento { get; set; }

        public byte[]? Foto { get; set; }

        public string? Apelido { get; set; }

        public string Idade
        {
            get
            {
                if (!DataNascimento.HasValue)
                    return "--";

                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Value.Year;
                if (DataNascimento.Value.Date > hoje.AddYears(-idade))
                    idade--;

                return $"{idade} anos";
            }
        }
    }
}