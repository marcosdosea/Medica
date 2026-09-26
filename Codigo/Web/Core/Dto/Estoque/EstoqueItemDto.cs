using System;
using System.Collections.Generic;

namespace Core.Dto.Estoque
{
    public class EstoqueItemDto
    {
        public int Id { get; set; }
        public uint IdMedicamento { get; set; }
        public string NomeMedicamento { get; set; } = string.Empty;
        public string FormaFarmaceutica { get; set; } = string.Empty;
        public List<uint> IdsPacientes { get; set; } = new();
        public string PacientesNomes { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public int QuantidadeMinima { get; set; }
        public DateTime DataValidade { get; set; }
        public string DataValidadeFormatada { get; set; } = string.Empty;
        public string DataValidadeIso { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
