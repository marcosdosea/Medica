using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicaWeb.Models
{
    public class GerenciarEstoqueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecione ao menos um paciente.")]
        [MinLength(1, ErrorMessage = "Selecione ao menos um paciente.")]
        [Display(Name = "Pacientes")]
        public List<uint> IdsPacientes { get; set; } = new();

        [Required(ErrorMessage = "Selecione um medicamento.")]
        [Range(1, uint.MaxValue, ErrorMessage = "Selecione um medicamento.")]
        [Display(Name = "Medicamento")]
        public uint IdMedicamento { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser de no mínimo 1.")]
        [Display(Name = "Quantidade")]
        public int? Quantidade { get; set; }

        [Required(ErrorMessage = "A quantidade mínima é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade mínima não pode ser negativa.")]
        [Display(Name = "Quantidade Mínima")]
        public int? QuantidadeMinima { get; set; }

        [Required(ErrorMessage = "A data de validade é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Validade")]
        public DateTime? DataValidade { get; set; }
    }
}
