using Core.Dto;
using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class MedicamentoViewModel
    {
        [Display(Name = "Código")]
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome do medicamento é obrigatório")]
        [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Apelido")]
        public string? Apelido { get; set; }

        [Display(Name = "Forma Farmacêutica")]
        [Required(ErrorMessage = "A forma farmacêutica é obrigatória")]
        public string FormaFarmaceutica { get; set; } = null!;

        [Display(Name = "Foto do Medicamento")]
        public byte[]? Foto { get; set; }


        [Required(ErrorMessage = "Selecione pelo menos um paciente")]
        [MinLength(1, ErrorMessage = "Você deve selecionar pelo menos um paciente")]
        public uint[]? PacientesSelecionados { get; set; }

        public IEnumerable<PacienteDto>? Pacientes { get; set; }
    }
}
