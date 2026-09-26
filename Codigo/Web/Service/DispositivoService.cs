using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class DispositivoService : IDispositivoService
    {
        private readonly MedicaContext context;

        public DispositivoService(MedicaContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Dispositivopaciente>> GetAll(uint idCuidador)
        {
            var pacientes = await context.Pacientes
                .AsNoTracking()
                .Where(p => p.Vinculos.Any(v => v.IdCuidador == idCuidador))
                .Include(p => p.Dispositivopacientes)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            var lista = new List<Dispositivopaciente>();
            foreach (var paciente in pacientes)
            {
                var dispositivo = paciente.Dispositivopacientes.FirstOrDefault()
                    ?? new Dispositivopaciente
                    {
                        IdPaciente = paciente.Id,
                        IdPacienteNavigation = paciente
                    };

                dispositivo.IdPacienteNavigation = paciente;
                lista.Add(dispositivo);
            }

            return lista;
        }
    }
}
