using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Execucao
{
    public enum Status
    {
        [Display(Name = "Sucesso")]
        SUCESSO,

        [Display(Name = "Atraso")]
        ATRASO,

        [Display(Name = "Falha")]
        FALHA
    }
}