using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Vinculo
{
    public enum Parentesco
    {
        [Display(Name = "Pai")]
        PAI,

        [Display(Name = "Mãe")]
        MAE,

        [Display(Name = "Filho")]
        FILHO,

        [Display(Name = "Cônjuge")]
        CONJUGE,

        [Display(Name = "Irmão")]
        IRMAO,

        [Display(Name = "Avô/Avó")]
        AVO,

        [Display(Name = "Tio")]
        TIO,

        [Display(Name = "Sobrinho")]
        SOBRINHO,

        [Display(Name = "Outros")]
        OUTROS
    }
}