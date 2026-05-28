using System.ComponentModel.DataAnnotations;

namespace Core.Enums
{
    public enum FormaFarmaceutica
    {        [Display(Name = "Comprimido")]
        COMPRIMIDO,

        [Display(Name = "Cápsula")]
        CAPSULA,

        [Display(Name = "Solução Oral")]
        SOLUCAO_ORAL,

        [Display(Name = "Creme")]
        CREME,

        [Display(Name = "Pomada")]
        POMADA,

        [Display(Name = "Injetável")]
        INJETAVEL,

        [Display(Name = "Supositório")]
        SUPOSITORIO
    }
}