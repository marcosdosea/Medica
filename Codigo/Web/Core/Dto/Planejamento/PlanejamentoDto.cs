using Core.Enum.Planejamento;

namespace Core.Dto.Planejamento
{
    public class PlanejamentoDto
    {
        public uint Id { get; set; }

        public string NomePaciente { get; set; } = null!;

        public string NomeMedicamento { get; set; } = null!;

        public Status Status { get; set; }

        public string Ativo { get; set; } = null!;
    }
}