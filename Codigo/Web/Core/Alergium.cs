using System;
using System.Collections.Generic;

namespace Core;

public partial class Alergium
{
    public uint Id { get; set; }

    public string Descricao { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public uint IdPaciente { get; set; }

    public uint? IdMedicamento { get; set; }

    public virtual Medicamento? IdMedicamentoNavigation { get; set; }

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
