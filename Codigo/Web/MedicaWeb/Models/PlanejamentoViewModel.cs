using Core.Enum;
using Core.Enum.Planejamento;
using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class PlanejamentoViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "O paciente é obrigatório.")]
        public uint IdPaciente { get; set; }

        [Display(Name = "Medicamento")]
        [Required(ErrorMessage = "O medicamento é obrigatório.")]
        public uint IdMedicamento { get; set; }

        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [Required(ErrorMessage = "A data de fim é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        [Display(Name = "Dias da Semana")]
        [Required(ErrorMessage = "Selecione ao menos um dia da semana.")]
        [StringLength(7, ErrorMessage = "Os dias da semana devem ter no máximo 7 caracteres.")]
        public string DiaSemana { get; set; } = null!;

        [Display(Name = "Hora do Medicamento")]
        [Required(ErrorMessage = "A hora é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [Display(Name = "Dosagem")]
        [Required(ErrorMessage = "A dosagem é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A dosagem deve ser um número maior que zero.")]
        public int Dosagem { get; set; }

        [Display(Name = "Unidade de Dosagem")]
        [Required(ErrorMessage = "A unidade de dosagem é obrigatória.")]
        public UnidadeDosagem Unidade { get; set; }

        [Display(Name = "Status Ativo")]
        [Required(ErrorMessage = "O status ativo é obrigatório.")]
        public StatusAtivo Ativo { get; set; } = StatusAtivo.S;
    }
}