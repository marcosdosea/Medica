namespace Core.Service
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAsync();
        Task<Paciente?> GetAsync(uint id);
        Task<IEnumerable<Paciente>> GetByMedicamentoAsync(uint idMedicamento);
        Task<uint> CreateAsync(Paciente paciente);
        Task EditAsync(Paciente paciente);
        Task DeleteAsync(uint id);
    }
}
