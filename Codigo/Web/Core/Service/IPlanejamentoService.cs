namespace Core.Service
{
    public interface IPlanejamentoService
    {
        Task<IEnumerable<Planejamento>> GetAll();
        Task<Planejamento?> Get(uint id);
        Task<uint> Create(Planejamento planejamento);
        Task Edit(Planejamento planejamento);
        Task Delete(uint id);
    }
}
