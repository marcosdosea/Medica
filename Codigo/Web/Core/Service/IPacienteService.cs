using Core.Dto.PacienteDto;
using Microsoft.AspNetCore.Mvc;

namespace Core.Service
{
    public interface IPacienteService
    {
        Task<IEnumerable<Paciente>> GetAsync(string searchTerm = "");
        Task<Paciente?> GetAsync(uint id);
        Task<IEnumerable<Paciente>> GetByMedicamentoAsync(uint idMedicamento);
        Task<uint> CreateAsync(Paciente paciente);
        Task EditAsync(Paciente paciente);
        Task DeleteAsync(uint id);
    }
}