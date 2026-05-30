using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Alergia
{
    public enum TipoAlergia
    {
        [Display(Name = "Medicamentosa")]
        MEDICAMENTO,

        [Display(Name = "Alimentar")]
        ALIMENTAR,

        [Display(Name = "Respiratória")]
        RESPIRATORIA,

        [Display(Name = "Por Contato")]
        CONTATO,

        [Display(Name = "Outros")]
        OUTROS
    }
}