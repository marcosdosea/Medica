namespace Core.Service
{
    public interface ICuidadorService
    {
        Task<uint> Create(Cuidador cuidador);

        Task<IEnumerable<Cuidador>> GetAll();

        Task<uint> GetIdByCpf(string cpf);
    }
}
