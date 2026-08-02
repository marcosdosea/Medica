using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Planejamento
{
    public enum UnidadeDosagem
    {
        [Display(Name = "Mililitro")]
        ML,

        [Display(Name = "Miligrama")]
        MG,

        [Display(Name = "Gramas")]
        G,

        [Display(Name = "Unidades Internacionais")]
        UI
    }
}