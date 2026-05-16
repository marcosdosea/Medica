using System;
using System.Collections.Generic;

namespace Core;

public partial class Paciente
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string? CartaoSus { get; set; }

    public string? TipoSanguineo { get; set; }

    public float? Peso { get; set; }

    public float? Altura { get; set; }

    /// <summary>
    /// &apos;M&apos; = Masculino;
    /// &apos;F&apos; = Feminino.
    /// </summary>
    public string Sexo { get; set; } = null!;

    public string? Apelido { get; set; }

    public sbyte AlergiaMedicamento { get; set; }

    public string Escolaridade { get; set; } = null!;

    public sbyte PossuiDeficiencia { get; set; }

    public string Cep { get; set; } = null!;

    public string Rua { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string? Identificador { get; set; }

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? Complemento { get; set; }

    public string Ddd { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string? DddResponsavel { get; set; }

    public string? TelefoneResponsavel { get; set; }

    public string? NomeTelefoneResponsavel { get; set; }

    public byte[]? Foto { get; set; }

    public virtual ICollection<Alergium> Alergia { get; set; } = [];

    public virtual ICollection<Deficiencium> Deficiencia { get; set; } = [];

    public virtual ICollection<Prescricao> Prescricaos { get; set; } = new List<Prescricao>();
}
