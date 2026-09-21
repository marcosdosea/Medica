namespace Core.Service
{
    public interface IDispositivoService
    {
        Task<string?> ObterToken(uint idPaciente);
    }
}
