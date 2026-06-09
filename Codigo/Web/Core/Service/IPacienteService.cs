namespace Core.Service
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAll();
        Task<Paciente?> Get(uint id);
        Task<uint> Create(Paciente paciente);
        Task Edit(Paciente paciente);
        Task Delete(uint id);
        Task Activate(uint id);
    }
}
