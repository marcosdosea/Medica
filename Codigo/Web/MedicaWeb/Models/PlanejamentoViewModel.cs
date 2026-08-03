using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class PlanejamentoViewModel
    {
        public uint Id { get; set; }

        public uint IdPaciente { get; set; }

        public uint IdMedicamento { get; set; }

        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }

        [Display(Name = "Dia da Semana")]
        public string DiaSemana { get; set; } = null!;

        [Display(Name = "Hora de Início")]
        [Required(ErrorMessage = "A hora de início é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [Display(Name = "Intervalo de Execução")]
        [DataType(DataType.Time)]
        public TimeSpan IntervaloExecucao { get; set; }

        [Display(Name = "Dosagem")]
        [Required(ErrorMessage = "A dosagem é obrigatória.")]
        public int Dosagem { get; set; }

        [Display(Name = "Unidade de Dosagem")]
        public string Unidade { get; set; } = null!;

        [Display(Name = "Ativo")]
        public string Ativo { get; set; } = "S";
    }
}
