using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class CuidadorViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(60, ErrorMessage = "O nome deve ter no máximo 60 caracteres.")]
        public string Nome { get; set; } = null!;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, ErrorMessage = "O CPF deve ter no máximo 11 caracteres.")]
        public string Cpf { get; set; } = null!;

        [Display(Name = "Foto")]
        public byte[]? Foto { get; set; }
    }
}
