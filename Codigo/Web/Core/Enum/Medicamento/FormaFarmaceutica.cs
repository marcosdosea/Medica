using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Medicamento
{
    public enum FormaFarmaceutica
    {
        [Display(Name = "Comprimido")]
        COMPRIMIDO,

        [Display(Name = "Cápsula")]
        CAPSULA,

        [Display(Name = "Gota")]
        GOTA,

        [Display(Name = "Solução Oral")]
        SOLUCAO_ORAL,

        [Display(Name = "Injetável")]
        INJETAVEL,

        [Display(Name = "Creme / Pomada")]
        CREME_POMADA,

        [Display(Name = "Colírio")]
        COLIRIO,

        [Display(Name = "Spray / Inalatório")]
        SPRAY_INALATORIO,

        [Display(Name = "Adesivo")]
        ADESIVO,

        [Display(Name = "Supositório")]
        SUPOSITORIO
    }
}