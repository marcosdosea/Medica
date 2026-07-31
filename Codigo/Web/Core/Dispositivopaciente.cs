using System;
using System.Collections.Generic;

namespace Core;

public partial class Dispositivopaciente
{
    public int Id { get; set; }

    public string FcmToken { get; set; } = null!;

    public DateTime DataAtualizacao { get; set; }

    public uint IdPaciente { get; set; }

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
