using System.ComponentModel.DataAnnotations;

namespace Core.Enums
{
    public enum Escolaridade
    {
        [Display(Name = "Analfabeto")]
        ANALFABETO,

        [Display(Name = "Fundamental Incompleto")]
        FUNDAMENTAL_INCOMPLETO,

        [Display(Name = "Fundamental Completo")]
        FUNDAMENTAL_COMPLETO,

        [Display(Name = "Médio Incompleto")]
        MEDIO_INCOMPLETO,

        [Display(Name = "Médio Completo")]
        MEDIO_COMPLETO,

        [Display(Name = "Superior Incompleto")]
        SUPERIOR_INCOMPLETO,

        [Display(Name = "Superior Completo")]
        SUPERIOR_COMPLETO,

        [Display(Name = "Pós-Graduação")]
        POS_GRADUACAO,

        [Display(Name = "Mestrado")]
        MESTRADO,

        [Display(Name = "Doutorado")]
        DOUTORADO
    }
}