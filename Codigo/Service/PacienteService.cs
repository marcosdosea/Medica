using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PacienteService : IPacienteService
    {

        private readonly MedicaContext context;

        public PacienteService(MedicaContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Buscar todos os pacientes cadastrados
        /// </summary>
        /// <returns>Lista de pacientes</returns>
        public IEnumerable<Paciente> GetAll()
        {
            return context.Pacientes
                    .AsNoTracking()
                    .OrderBy(p => p.Nome)
                    .ToList();
        }

        /// <summary>
        /// Buscar todos os pacientes cadastrados por medicamento
        /// </summary>
        /// <returns>Lista de pacientes</returns>
        public IEnumerable<Paciente> GetByMedicamento(uint idMedicamento)
        {
            return context.Prescricaos
                    .Where(p => p.IdMedicamento == idMedicamento)
                    .Select(p => p.IdPacienteNavigation)
                    .Distinct()
                    .ToList();
        }
    }
}
