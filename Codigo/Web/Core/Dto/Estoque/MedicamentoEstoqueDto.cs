namespace Core.Dto.Estoque
{
    public class MedicamentoEstoqueDto
    {
        public uint IdPaciente { get; set; }
        public uint IdMedicamento { get; set; }
        public string NomeMedicamento { get; set; } = string.Empty;
        public string FormaFarmaceutica { get; set; } = string.Empty;
    }
}
