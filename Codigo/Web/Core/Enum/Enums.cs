using System.ComponentModel.DataAnnotations;
namespace Core.Enums
{

    public enum Sexo
    {
        [Display (Name = "Masculino")] M,
        [Display (Name = "Feminino")] F
    }
    public enum TipoSanguineo
    {
        [Display(Name = "A+")] APositivo,
        [Display(Name = "A-")] ANegativo,
        [Display(Name = "B+")] BPositivo,
        [Display(Name = "B-")] BNegativo,
        [Display(Name = "AB+")] ABPositivo,
        [Display(Name = "AB-")] ABNegativo,
        [Display(Name = "O+")] OPositivo,
        [Display(Name = "O-")] ONegativo
    }

        public enum EstadoBr
        {
            [Display(Name = "AC")] AC,
            [Display(Name = "AL")] AL,
            [Display(Name = "AP")] AP,
            [Display(Name = "AM")] AM,
            [Display(Name = "BA")] BA,
            [Display(Name = "CE")] CE,
            [Display(Name = "DF")] DF,
            [Display(Name = "ES")] ES,
            [Display(Name = "GO")] GO,
            [Display(Name = "MA")] MA,
            [Display(Name = "MT")] MT,
            [Display(Name = "MS")] MS,
            [Display(Name = "MG")] MG,
            [Display(Name = "PA")] PA,
            [Display(Name = "PB")] PB,
            [Display(Name = "PR")] PR,
            [Display(Name = "PE")] PE,
            [Display(Name = "PI")] PI,
            [Display(Name = "RJ")] RJ,
            [Display(Name = "RN")] RN,
            [Display(Name = "RS")] RS,
            [Display(Name = "RO")] RO,
            [Display(Name = "RR")] RR,
            [Display(Name = "SC")] SC,
            [Display(Name = "SP")] SP,
            [Display(Name = "SE")] SE,
            [Display(Name = "TO")] TO
        }

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