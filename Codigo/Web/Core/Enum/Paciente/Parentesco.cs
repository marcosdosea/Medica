using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Paciente
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

        [Display(Name = "Irmão(ã)")]
        IRMAO,

        [Display(Name = "Avô(ó)")]
        AVO,

        [Display(Name = "Tio(a)")]
        TIO,

        [Display(Name = "Sobrinho(a)")]
        SOBRINHO,

        [Display(Name = "Outros")]
        OUTROS
    }
}