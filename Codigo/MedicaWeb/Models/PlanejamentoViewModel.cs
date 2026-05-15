using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class PlanejamentoViewModel
    {
        public uint IdPaciente { get; set; }

        public uint IdMedicamento { get; set; }

        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }

        [Display(Name = "Hora de Início")]
        [Required(ErrorMessage = "A hora de início é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Display(Name = "Frequência")]
        [Required(ErrorMessage = "A frequência é obrigatória.")]
        public TimeSpan Frequencia { get; set; }

        [Display(Name = "Dosagem")]
        [Required(ErrorMessage = "A dosagem é obrigatória.")]
        [StringLength(30, ErrorMessage = "A dosagem deve ter no máximo 30 caracteres.")]
        public string Dosagem { get; set; } = null!;

        [Display(Name = "Data de Confirmação")]
        [DataType(DataType.Date)]
        public DateTime? DataConfirmacao { get; set; }

        [Display(Name = "Hora de Confirmação")]
        [DataType(DataType.Time)]
        public TimeSpan? HoraConfirmacao { get; set; }

        [Display(Name = "Latitude")]
        [Range(-90, 90, ErrorMessage = "Informe uma latitude válida.")]
        public decimal? Latitude { get; set; }

        [Display(Name = "Longitude")]
        [Range(-180, 180, ErrorMessage = "Informe uma longitude válida.")]
        public decimal? Longitude { get; set; }

        [Display(Name = "Foto")]
        public byte[]? Foto { get; set; }

    }
}
