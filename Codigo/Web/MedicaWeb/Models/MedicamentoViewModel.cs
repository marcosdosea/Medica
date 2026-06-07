using Core.Enum.Medicamento;
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
        public string? Apelido { get; set; }

        [Required(ErrorMessage = "O quantidade do medicamento é obrigatório")]
        public int Quantidade { get; set; }

        [Display(Name = "Forma Farmacêutica")]
        [Required(ErrorMessage = "A forma farmacêutica é obrigatória")]
        public FormaFarmaceutica FormaFarmaceutica { get; set; }

        [Display(Name = "Foto do Medicamento")]
        public byte[]? Foto { get; set; }

        public string Ativo { get; set; } = "S";

        public uint IdCuidador { get; set; }
    }
}
