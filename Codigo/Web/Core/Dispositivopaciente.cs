using System;
using System.Collections.Generic;

namespace Core;

public partial class Dispositivopaciente
{
    public uint Id { get; set; }

    public string FcmToken { get; set; } = null!;

    /// <summary>
    /// Utilizado para saber quando um dispositivo foi cadastrado ou alterado.
    /// </summary>
    public DateTime DataAtualizacao { get; set; }

    public uint IdPaciente { get; set; }

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
