namespace Core.Dto.Paciente
{
    public class PacienteMobileDto
    {
        public bool PossuiDeficiencia { get; set; }
        public string Sexo { get; set; } = null!;
        public string Escolaridade { get; set; } = null!;

        public List<DeficienciaMobileDto> Deficiencias { get; set; } = new();

        public class DeficienciaMobileDto
        {
            public string Descricao { get; set; } = null!;
        }
    }
}