using System;
using System.Collections.Generic;

namespace Core;

public partial class Vinculo
{
    public uint IdPaciente { get; set; }

    public uint IdCuidador { get; set; }

    public string Parentesco { get; set; } = null!;

    public virtual Cuidador IdCuidadorNavigation { get; set; } = null!;

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
