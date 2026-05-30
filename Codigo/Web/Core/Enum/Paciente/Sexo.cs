using System.ComponentModel.DataAnnotations;

namespace Core.Enum.Paciente
{
    public enum Sexo
    {
        [Display(Name = "Masculino")] 
        M,

        [Display(Name = "Feminino")] 
        F
    }
}