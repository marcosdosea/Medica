using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Vinculo
{
    public enum Parentesco
    {
        [Display(Name = "Pai")]
        PAI,

        [Display(Name = "Mãe")]
        MAE,

        [Display(Name = "Filho(a)")]
        FILHO,

        [Display(Name = "Cônjuge")]
        CONJUGE,

        [Display(Name = "Irmão/Irmã")]
        IRMAO,

        [Display(Name = "Avô/Avó")]
        AVO,

        [Display(Name = "Tio(a)")]
        TIO,

        [Display(Name = "Sobrinho(a)")]
        SOBRINHO,

        [Display(Name = "Outros")]
        OUTROS
    }
}