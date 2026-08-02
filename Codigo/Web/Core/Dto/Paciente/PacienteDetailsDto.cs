using Core.Enum.Alergia;
using Core.Enum.Paciente;

namespace Core.Dto.Paciente
{
    public class PacienteDetailsDto
    {
        public uint Id { get; set; }

        public string Nome { get; set; } = null!;

        public string Cpf { get; set; } = null!;

        public string? CartaoSus { get; set; }

        public TipoSanguineo TipoSanguineo { get; set; }

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

        public string? Ativo { get; set; }

        public DateTime? DataNascimento { get; set; }

        public VinculoDto Vinculo { get; set; } = null!;

        public List<PacienteDeficienciaDto> Deficiencias { get; set; } = [];

        public List<PacienteAlergiaDto> Alergias { get; set; } = [];

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

        public class VinculoDto
        {
            public string Parentesco { get; set; } = null!;
        }
    }
}