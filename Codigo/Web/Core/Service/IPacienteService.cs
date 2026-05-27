namespace Core.Service
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAll();
        Task<Paciente?> Get(uint id);
        Task<IEnumerable<Paciente>> GetByMedicamento(uint idMedicamento);
        Task<uint> Create(Paciente paciente);
        Task Edit(Paciente paciente);
        Task Delete(uint id);
    }
}
