using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Paciente
{
    public enum TipoSanguineo
    {
        [Display(Name = "A+")] 
        A_POSITIVO,

        [Display(Name = "A-")] 
        A_NEGATIVO,

        [Display(Name = "B+")] 
        B_POSITIVO,
        
        [Display(Name = "B-")] 
        B_NEGATIVO,
        [Display(Name = "AB+")] 
        AB_POSITIVO,

        [Display(Name = "AB-")] 
        AB_NEGATIVO,
        
        [Display(Name = "O+")] 
        O_POSITIVO,

        [Display(Name = "O-")] 
        O_NEGATIVO
    }
}