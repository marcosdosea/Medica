namespace Core.Dto.Execucao
{
    public class ExecucaoDto
    {
        public uint Id { get; set; }
        public DateTime DataConfirmacao { get; set; }
        public TimeSpan? HoraConfirmacao { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Status { get; set; } = "SUCESSO";
        public int IdPlanejamento { get; set; }
    }

    public class ExecucaoRequestDto
    {
        public uint IdPlanejamento { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Recebendo como string para facilitar o bind do JSON enviado pelo Flutter
        public string DataConfirmacao { get; set; } = null!;
        public string? HoraConfirmacao { get; set; }
    }
}
