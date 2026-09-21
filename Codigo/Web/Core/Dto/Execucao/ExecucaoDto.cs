namespace Core.Dto.Execucao
{
    public class ExecucaoDto
    {
        public uint Id { get; set; }
        public DateTime? DataConfirmacao { get; set; }
        public TimeSpan? HoraConfirmacao { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Status { get; set; } = null!;
        public int IdPlanejamento { get; set; }
    }

    public class ExecucaoRequestDto
    {
        public uint IdPlanejamento { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? DataConfirmacao { get; set; }

        public string? HoraConfirmacao { get; set; }
    }
}
