using System;
using System.Collections.Generic;

namespace Core;

public partial class Planejamento
{
    public uint Id { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public TimeSpan Frequencia { get; set; }

    public string Dosagem { get; set; } = null!;

    public TimeSpan HoraInicio { get; set; }

    public TimeSpan? HoraAtual { get; set; }

    public DateTime? DataConfirmacao { get; set; }

    public TimeSpan? HoraConfirmacao { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public byte[]? Foto { get; set; }

    public uint IdPaciente { get; set; }

    public uint IdMedicamento { get; set; }

    public virtual Prescricao Prescricao { get; set; } = null!;
}
