using Core.Enum.Medicamento;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Util;

namespace MedicaWeb.Models
{
    public class MedicamentoViewModel
    {
        [Display(Name = "Código")]
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório.")]
        [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Apelido")]
        public string? Apelido { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public int Quantidade { get; set; }

        [Display(Name = "Forma Farmacêutica")]
        [Required(ErrorMessage = "Campo obrigatório.")]
        public FormaFarmaceutica FormaFarmaceutica { get; set; }

        [Display(Name = "Foto do Medicamento")]
        public byte[]? Foto { get; set; }

        [Display(Name = "Foto do Medicamento")]
        [Foto(TamanhoMaximoBytes = 65535)]
        public IFormFile? FotoMedicamento { get; set; }

        public bool RemoverFoto { get; set; } = false;

        public uint IdCuidador { get; set; }

        public string Ativo { get; set; } = "S";
    }
}
