using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class DeficienciumViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public string Descricao { get; set; } = null!;

        public uint IdPaciente { get; set; }
    }
}
