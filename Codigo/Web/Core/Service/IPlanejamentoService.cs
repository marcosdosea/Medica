namespace Core.Service
{
    public interface IPlanejamentoService
    {
        Task<IEnumerable<Planejamento>> GetAll(uint idCuidador, uint? idPaciente = null);
        Task<Planejamento?> Get(uint id);
        Task<bool> Create(IEnumerable<Planejamento> planejamentos);
        Task Edit(Planejamento planejamento);
        Task Delete(uint id);
        Task Activate(uint id);
    }
}
