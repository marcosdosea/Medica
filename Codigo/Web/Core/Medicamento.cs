using System;
using System.Collections.Generic;

namespace Core;

public partial class Medicamento
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Apelido { get; set; }

    public int Quantidade { get; set; }

    public string FormaFarmaceutica { get; set; } = null!;

    public byte[]? Foto { get; set; }

    public uint IdCuidador { get; set; }

    public string Ativo { get; set; } = null!;

    public virtual ICollection<Alergium> Alergia { get; set; } = new List<Alergium>();

    public virtual Cuidador IdCuidadorNavigation { get; set; } = null!;

    public virtual ICollection<Planejamento> Planejamentos { get; set; } = new List<Planejamento>();
}
