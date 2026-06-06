namespace Core.Service
{
    public interface IMedicamentoService
    {
        Task<Medicamento?> Get(uint id);
        Task<uint> Create(Medicamento medicamento);
        Task Edit(Medicamento medicamento);
        Task Delete(uint id);
        Task<IEnumerable<Medicamento>> GetAll();
    }
}
