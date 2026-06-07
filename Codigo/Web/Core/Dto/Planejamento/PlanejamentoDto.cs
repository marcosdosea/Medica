namespace Core.Dto.Planejamento
{
    public class PlanejamentoDto
    {
        public uint Id { get; set; }

        public string NomePaciente { get; set; } = null!;

        public string NomeMedicamento { get; set; } = null!;

        public string Ativo { get; set; } = null!;
    }
}