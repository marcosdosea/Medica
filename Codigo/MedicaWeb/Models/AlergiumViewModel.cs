using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class AlergiumViewModel
    {

        public uint IdPaciente { get; set; }

        public uint IdCuidador { get; set; }

        [Display(Name = "Parentesco")]
        [Required(ErrorMessage = "O parentesco é obrigatório.")]
        [StringLength(30, ErrorMessage = "O parentesco deve ter no máximo 30 caracteres.")]
        public string Parentesco { get; set; } = null!;
    }
}
