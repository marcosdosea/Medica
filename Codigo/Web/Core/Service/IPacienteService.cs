using Core.Dto.Paciente;

namespace Core.Service
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAll(uint idCuidador);
        Task<Paciente?> Get(uint id);
        Task<IEnumerable<Paciente>> GetByMedicamento(uint idMedicamento);
        Task<uint> Create(Paciente paciente, Vinculo vinculo);
        Task Edit(Paciente paciente);
        Task Delete(uint id);
        Task<IEnumerable<PacienteMobileDto>> GetMobileAsync();


    }
}
