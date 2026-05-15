using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public enum FormaFarmaceuticaEnum
    {
        [Display(Name = "Inalante")]
        Inalante,

        [Display(Name = "Injetável")]
        Injetavel,

        [Display(Name = "Creme")]
        Creme,

        [Display(Name = "Líquido")]
        Liquido,

        [Display(Name = "Supositório")]
        Supositorio,

        [Display(Name = "Ingerir")]
        Ingerir
    }

    public class MedicamentoViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Forma Farmacêutica")]
        [Required(ErrorMessage = "A forma farmacêutica é obrigatória.")]
        public FormaFarmaceuticaEnum? FormaFarmaceutica { get; set; }

        [Display(Name = "Apelido")]
        [StringLength(100, ErrorMessage = "O apelido deve ter no máximo 100 caracteres.")]
        public string? Apelido { get; set; }

        [Display(Name = "Foto")]
        public byte[]? Foto { get; set; }
    }
}