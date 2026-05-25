using Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Core.Service
{
    public interface IPacienteService
    {
        Task<IEnumerable<PacienteDto>> GetAsync(string searchTerm = "");
        Task<PacienteDetailsDto?> GetAsync(uint id);
        Task<IEnumerable<PacienteDto>> GetByMedicamentoAsync(uint idMedicamento);
        Task<uint> CreateAsync(PacienteDetailsDto pacienteDetailsDto);
        Task EditAsync(PacienteDetailsDto pacienteDetailsDto);
        Task DeleteAsync(uint id);
    }
}