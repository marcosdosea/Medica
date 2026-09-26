using System;
using System.Collections.Generic;

namespace Core;

public partial class Estoque
{
    public int Id { get; set; }

    public uint IdMedicamento { get; set; }

    public int Quantidade { get; set; }

    public int QuantidadeMinima { get; set; }

    public DateTime DataValidade { get; set; }

    /// <summary>
    /// INSUFICIENTE = Acabou; BAIXO = ACABANDO; REGULAR = DA PARA O PLANEJAMENTO.
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual Medicamento IdMedicamentoNavigation { get; set; } = null!;

    public virtual ICollection<Paciente> IdPacientes { get; set; } = new List<Paciente>();
}
