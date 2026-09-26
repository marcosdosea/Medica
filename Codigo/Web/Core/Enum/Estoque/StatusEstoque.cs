using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Estoque
{
    public enum StatusEstoque
    {
        [Display(Name = "Regular")]
        REGULAR,

        [Display(Name = "Baixo")]
        BAIXO,

        [Display(Name = "Insuficiente")]
        INSUFICIENTE
    }
}
