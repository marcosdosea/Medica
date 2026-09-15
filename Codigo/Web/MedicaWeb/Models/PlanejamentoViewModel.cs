using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Enum.Planejamento;

namespace MedicaWeb.Models
{
    public class PlanejamentoViewModel
    {
        public uint Id { get; set; }

        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "Selecione um paciente.")]
        [Range(1, uint.MaxValue, ErrorMessage = "Selecione um paciente.")]
        public uint IdPaciente { get; set; }

        public uint IdMedicamento { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; } = DateTime.Today;

        [Display(Name = "Data de Fim")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; } = DateTime.MaxValue;

        public bool Continuo { get; set; } = false;

        public string DiaSemana { get; set; } = null!;

        [Display(Name = "Hora de Início")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        public TimeSpan IntervaloExecucao { get; set; }

        public int Dosagem { get; set; }

        public UnidadeDosagem Unidade { get; set; }

        public string Ativo { get; set; } = "S";

        public string Status { get; set; } = "NAO_INICIADO";

        [MinLength(1, ErrorMessage = "Adicione ao menos um planejamento antes de salvar.")]
        public List<PlanejamentoViewModel> Itens { get; set; } = new();
    }
}