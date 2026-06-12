using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Planejamento
{
    public enum Status
    {
        [Display(Name = "Não Iniciado")]
        NAO_INICIADO,

        [Display(Name = "Em Andamento")]
        EM_ANDAMENTO,

        [Display(Name = "Concluído")]
        CONCLUIDO,

        [Display(Name = "Interrompido")]
        INTERROMPIDO
    }
}