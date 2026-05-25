using System;
using System.Collections.Generic;

namespace Core;

public partial class Prescricao
{
    public uint IdPaciente { get; set; }

    public uint IdMedicamento { get; set; }

    public virtual Medicamento IdMedicamentoNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;

    public virtual ICollection<Planejamento> Planejamentos { get; set; } = new List<Planejamento>();
}
