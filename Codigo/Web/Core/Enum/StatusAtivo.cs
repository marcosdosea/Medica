using System.ComponentModel.DataAnnotations;

namespace Core.Enum
{
    public enum StatusAtivo
    {
        [Display(Name = "Ativo")]
        S,

        [Display(Name = "Inativo")]
        N
    }
}
