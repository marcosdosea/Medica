namespace Core.Dto.Dispositivo
{
    public class DispositivoDto
    {
        public uint IdPaciente { get; set; }

        public string NomePaciente { get; set; } = string.Empty;

        public int QuantidadeDispositivos { get; set; }
    }
}
