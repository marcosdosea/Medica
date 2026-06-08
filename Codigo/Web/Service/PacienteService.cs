using Core;
using Core.Enum;
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

        public async Task<uint> Create(Paciente paciente)
        {
            await context.Pacientes.AddAsync(paciente);
            await context.SaveChangesAsync();
            return paciente.Id;
        }

        public async Task Edit(Paciente paciente)
        {
            context.Pacientes.Update(paciente);
            await context.SaveChangesAsync();
        }

        public async Task Delete(uint id)
        {
            var paciente = await this.Get(id);

            bool possuiExecucoes = await context.Planejamentos
                                                .AnyAsync(pl => pl.IdPaciente == id && pl.Execucaos.Any());

            if (possuiExecucoes)
            {
                paciente!.Ativo = StatusAtivo.N.ToString();
                context.Pacientes.Update(paciente);
            }
            else
            {
                if (paciente!.Vinculos != null && paciente.Vinculos.Count != 0)
                {
                    context.Vinculos.RemoveRange(paciente.Vinculos);
                }

                if (paciente!.Planejamentos != null && paciente.Planejamentos.Count != 0)
                {
                    context.Planejamentos.RemoveRange(paciente.Planejamentos);
                }

                if (paciente.Alergia != null && paciente.Alergia.Count != 0)
                {
                    context.Alergia.RemoveRange(paciente.Alergia);
                }

                if (paciente.Deficiencia != null && paciente.Deficiencia.Count != 0)
                {
                    context.Deficiencia.RemoveRange(paciente.Deficiencia);
                }

                context.Pacientes.Remove(paciente);
            }

            await context.SaveChangesAsync();
        }

        public async Task<Paciente?> Get(uint id)
        {
            return await context.Pacientes
                                .Include(p => p.Alergia)
                                    .ThenInclude(a => a.IdMedicamentoNavigation)
                                .Include(p => p.Deficiencia)
                                .Include(p => p.Planejamentos)
                                    .ThenInclude(pl => pl.Execucaos)
                                .Include(p => p.Vinculos) 
                                    .ThenInclude(v => v.IdCuidadorNavigation)
                                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Paciente>> GetByMedicamento(uint idMedicamento)
        {
            return await context.Planejamentos
                .AsNoTracking()
                .Where(p => p.IdMedicamento == idMedicamento)
                .Select(p => p.IdPaciente)
                .Distinct()
                .Join(
                    context.Pacientes.AsNoTracking(),
                    idPaciente => idPaciente,
                    paciente => paciente.Id,
                    (idPaciente, paciente) => paciente
                )
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<Paciente>> GetAll()
        {
            var query = context.Pacientes.AsNoTracking();
            return await query.ToListAsync();
        }
    }
}