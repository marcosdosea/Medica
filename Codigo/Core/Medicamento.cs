using System;
using System.Collections.Generic;

namespace Core;

public partial class Medicamento
{
    public uint Id { get; set; }

    public string FormaFarmaceutica { get; set; } = null!;

    public string? Apelido { get; set; }

    public byte[]? Foto { get; set; }

    public virtual ICollection<Planejamento> Planejamentos { get; set; } = [];
}
