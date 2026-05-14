using System;
using System.Collections.Generic;

namespace Core;

public partial class Deficiencium
{
    public uint Id { get; set; }

    public string Descricao { get; set; } = null!;

    public uint IdPaciente { get; set; }

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
