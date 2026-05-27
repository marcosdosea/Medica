using System.ComponentModel.DataAnnotations;
namespace Core.Enums
{ 
    public enum Escolaridade
    {
        [Display(Name = "Analfabeto")]
        Analfabeto,

        [Display(Name = "Fundamental Incompleto")]
        FundamentalIncompleto,

        [Display(Name = "Fundamental Completo")]
        FundamentalCompleto,

        [Display(Name = "Médio Incompleto")]
        MedioIncompleto,

        [Display(Name = "Médio Completo")]
        MedioCompleto,

        [Display(Name = "Superior Incompleto")]
        SuperiorIncompleto,

        [Display(Name = "Superior Completo")]
        SuperiorCompleto,

        [Display(Name = "Pós-Graduação")]
        PosGraduacao,

        [Display(Name = "Mestrado")]
        Mestrado,

        [Display(Name = "Doutorado")]
        Doutorado
    }
}