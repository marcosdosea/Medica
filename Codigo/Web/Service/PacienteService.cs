using Core;
using Core.Dto.Paciente;
using Core.Enum;
using Core.Enum.Paciente;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class PacienteService : IPacienteService
    {
        private readonly MedicaContext context;

        public PacienteService(MedicaContext context)
        {
            this.context = context;
        }

        public async Task<uint> Create(Paciente paciente, Vinculo vinculo)
        {
            paciente.Vinculos.Add(vinculo);
            await context.Pacientes.AddAsync(paciente);
            await context.SaveChangesAsync();
            return paciente.Id;
        }

        public async Task Edit(Paciente paciente)
        {
            context.Pacientes.Update(paciente);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove ou inativa o paciente e seus planejamentos conforme o histórico
        /// </summary>
        /// <param name="id">Id do paciente</param>
        public async Task Delete(uint id)
        {
            var paciente = await context.Pacientes.FindAsync((int)id);
            if (paciente == null)
            {
                throw new ServiceException("Paciente não encontrado.");
            }

            var planejamentos = await context.Planejamentos
                .Where(p => p.IdPaciente == id)
                .ToListAsync();
            bool possuiHistorico = planejamentos.Any();

            if (possuiHistorico)
            {
                paciente.Ativo = StatusAtivo.N.ToString();
                context.Pacientes.Update(paciente);

                foreach (var p in planejamentos.Where(p => p.Ativo == StatusAtivo.S.ToString()))
                {
                    p.Ativo = StatusAtivo.N.ToString();
                    if (p.Status == Core.Enum.Planejamento.Status.EM_ANDAMENTO.ToString())
                    {
                        p.Status = Core.Enum.Planejamento.Status.INTERROMPIDO.ToString();
                    }
                }

                context.Planejamentos.UpdateRange(planejamentos);
            }
            else
            {
                var vinculos = await context.Vinculos
                    .Where(v => v.IdPaciente == id)
                    .ToListAsync();
                if (vinculos.Any())
                {
                    context.Vinculos.RemoveRange(vinculos);
                }

                var dispositivos = await context.Dispositivopacientes
                    .Where(d => d.IdPaciente == id)
                    .ToListAsync();
                if (dispositivos.Any())
                {
                    context.Dispositivopacientes.RemoveRange(dispositivos);
                }

                var alergias = await context.Alergia
                    .Where(a => a.IdPaciente == id)
                    .ToListAsync();
                if (alergias.Any())
                {
                    context.Alergia.RemoveRange(alergias);
                }

                context.Pacientes.Remove(paciente);
            }

            await context.SaveChangesAsync();
        }

        public async Task<Paciente?> Get(uint id)
        {
            return await context.Pacientes
                                .AsNoTracking()
                                .Include(p => p.Alergia)
                                    .ThenInclude(a => a.IdMedicamentoNavigation)
                                .Include(p => p.Vinculos)
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

        public async Task<IEnumerable<Paciente>> GetAll(uint idCuidador)
        {
            return await context.Pacientes
                .AsNoTracking()
                .Where(p => p.Vinculos.Any(v => v.IdCuidador == idCuidador))
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<PacienteMobileDto>> GetMobileAsync()
        {
            var pacientesDoBanco = await context.Pacientes
                .Select(p => new
                {
                    p.Id,
                    p.Escolaridade,
                    p.PossuiDeficiencia
                })
                .ToListAsync();

            var resultadoDto = pacientesDoBanco.Select(p => new PacienteMobileDto
            {
                Id = p.Id,
                Escolaridade = p.Escolaridade.ToString(),
                PossuiDeficiencia = p.PossuiDeficiencia == 1
            }).ToList();

            return resultadoDto;
        }
    }
}