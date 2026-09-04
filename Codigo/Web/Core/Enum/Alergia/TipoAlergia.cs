using System.ComponentModel.DataAnnotations;

namespace Core.Enum.TipoAlergia
{
    public enum TipoAlergia
    {
        [Display(Name = "Medicamento")]
        MEDICAMENTO,

        [Display(Name = "Alimentar")]
        ALIMENTAR,

        [Display(Name = "Respiratória")]
        RESPIRATORIA,

        [Display(Name = "Contato")]
        CONTATO,

        [Display(Name = "Outros")]
        OUTROS
    }
}