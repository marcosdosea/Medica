using Core.Enum.Paciente;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Paciente
{
    public class PacienteDetailsDto
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Cpf { get; set; } = null!;

        public string? CartaoSus { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public TipoSanguineo? TipoSanguineo { get; set; }

        public float? Peso { get; set; }

        public float? Altura { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Sexo { get; set; } = null!;

        public string? Apelido { get; set; }

        public sbyte AlergiaMedicamento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Escolaridade { get; set; } = null!;

        public sbyte PossuiDeficiencia { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Cep { get; set; } = null!;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Rua { get; set; } = null!;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Bairro { get; set; } = null!;

        public string? Identificador { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Cidade { get; set; } = null!;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Estado { get; set; } = null!;

        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Ddd { get; set; } = null!;

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Telefone { get; set; } = null!;

        public string? DddResponsavel { get; set; }

        public string? TelefoneResponsavel { get; set; }

        public string? NomeTelefoneResponsavel { get; set; }

        public IFormFile? Foto { get; set; }

        public string? Ativo { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public DateTime? DataNascimento { get; set; }

        public VinculoDto Vinculo { get; set; } = new();

        public List<PacienteDeficienciaDto> Deficiencias { get; set; } = [];

        public List<PacienteAlergiaDto> Alergias { get; set; } = [];

        public string? Deficiencia { get; set; }


        public class PacienteDeficienciaDto
        {
            public string Descricao { get; set; } = null!;
        }

        public class PacienteAlergiaDto
        {
            public uint? IdMedicamento { get; set; }
            public string? MedicamentoNome { get; set; }
            public string Tipo { get; set; } = null!;
            public string? Descricao { get; set; }
        }

        public class VinculoDto
        {
            public int IdCuidador { get; set; }

            [Required(ErrorMessage = "Campo obrigatório.")]
            public Parentesco Parentesco { get; set; }
        }
    }
}