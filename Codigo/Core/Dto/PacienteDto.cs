namespace Core.Dto
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

    public class PacienteDetailsDto
    {
        public uint Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string? CartaoSus { get; set; }
        public string? TipoSanguineo { get; set; }
        public float? Peso { get; set; }
        public float? Altura { get; set; }
        public string Sexo { get; set; } = null!;
        public string? Apelido { get; set; }
        public sbyte AlergiaMedicamento { get; set; }
        public string Escolaridade { get; set; } = null!;
        public sbyte PossuiDeficiencia { get; set; }
        public string Cep { get; set; } = null!;
        public string Rua { get; set; } = null!;
        public string Bairro { get; set; } = null!;
        public string? Identificador { get; set; }
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string? Complemento { get; set; }
        public string Ddd { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string? DddResponsavel { get; set; }
        public string? TelefoneResponsavel { get; set; }
        public string? NomeTelefoneResponsavel { get; set; }
        public byte[]? Foto { get; set; }
        public DateTime? DataNascimento { get; set; }
    }
}