using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class MedicamentoViewModel
    {
        [Display(Name = "Código")]
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome do medicamento é obrigatório")]
        [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Apelido")]
        [Required(ErrorMessage = "O apelido do medicamento é obrigatório")]
        [StringLength(60, ErrorMessage = "O apelido deve ter no máximo 60 caracteres")]
        public string Apelido { get; set; } = null!;

        [Display(Name = "Forma Farmacêutica")]
        [Required(ErrorMessage = "A forma farmacêutica é obrigatória")]
        public string FormaFarmaceutica { get; set; } = null!;

        [Display(Name = "Foto do Medicamento")]
        public byte[]? Foto { get; set; }
    }
}
