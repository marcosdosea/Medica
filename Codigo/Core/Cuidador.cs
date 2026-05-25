using System;
using System.Collections.Generic;

namespace Core;

public partial class Cuidador
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public byte[]? Foto { get; set; }

    public virtual ICollection<Vinculo> Vinculos { get; set; } = new List<Vinculo>();
}
