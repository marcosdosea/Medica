using Core.Enum.Alergia;

namespace Core.Dto.PacienteDto
{
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

        public List<PacienteDeficienciaDto> Deficiencias { get; set; } = new();

        public List<PacienteAlergiaDto> Alergias { get; set; } = new();

        public class PacienteDeficienciaDto
        {
            public string Descricao { get; set; } = null!;
        }

        public class PacienteAlergiaDto
        {
            public int Id { get; set; }

            public string Descricao { get; set; } = null!;

            public TipoAlergia Tipo { get; set; }

            public int? IdMedicamento { get; set; }

            public string? MedicamentoNome { get; set; }
        }
    }
}