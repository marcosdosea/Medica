namespace Core.Service
{
    public interface IDispositivoService
    {
        Task<IEnumerable<Dispositivopaciente>> GetAll(uint idCuidador);
    }
}
