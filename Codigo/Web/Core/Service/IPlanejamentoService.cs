namespace Core.Service
{
    public interface IPlanejamentoService
    {
        Task<IEnumerable<Planejamento>> GetAll(uint idCuidador);
        Task<IEnumerable<Planejamento>> GetAllByPaciente(uint idPaciente, DateTime? ultimaSincronizacao = null);
        Task<IEnumerable<int>> GetIdsExcluidosByPaciente(uint idPaciente, DateTime? ultimaSincronizacao = null);
        Task<Planejamento?> Get(uint id);
        Task<bool> Create(IEnumerable<Planejamento> planejamentos);
        Task Edit(Planejamento planejamento);
        Task Delete(uint id);
        Task Activate(uint id);
    }
}
