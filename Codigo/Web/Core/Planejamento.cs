using System;
using System.Collections.Generic;

namespace Core;

public partial class Planejamento
{
    public int Id { get; set; }

    public uint IdPaciente { get; set; }

    public uint IdMedicamento { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataFim { get; set; }

    /// <summary>
    /// DOM,SEG,TER,QUA,QUI,SEX,SAB
    /// </summary>
    public string DiaSemana { get; set; } = null!;

    public TimeSpan Hora { get; set; }

    public int Dosagem { get; set; }

    public string UnidadeDosagem { get; set; } = null!;

    public string Ativo { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Execucao> Execucaos { get; set; } = new List<Execucao>();

    public virtual Medicamento IdMedicamentoNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
