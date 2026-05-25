using System;
using System.Collections.Generic;

namespace Core;

public partial class Execucao
{
    /// <summary>
    /// Esta tabela guarda informações da execucao do planejamento das medicações.
    /// </summary>
    public uint Id { get; set; }

    public DateTime DataConfirmacao { get; set; }

    public TimeSpan HoraConfirmacao { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int IdPlanejamento { get; set; }

    public virtual Planejamento IdPlanejamentoNavigation { get; set; } = null!;
}
