using Core;
using Core.Enum;
using Core.Enum.Planejamento;
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
        /// Criar um novo paciente na base de dados
        /// </summary>
        /// <param name="paciente">Dados do paciente</param>
        /// <returns>Id do novo paciente</returns>
        public async Task<uint> Create(Paciente paciente)
        {
            await context.Pacientes.AddAsync(paciente);
            await context.SaveChangesAsync();
            return paciente.Id;
        }

        /// <summary>
        /// Atualizar dados de um paciente da base de dados
        /// </summary>
        /// <param name="paciente">Novos dados do paciente</param>
        public async Task Edit(Paciente paciente)
        {
            context.Pacientes.Update(paciente);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Remover dados de um paciente da base de dados
        /// </summary>
        /// <param name="id">id do paciente</param>
        public async Task Delete(uint id)
        {
            var paciente = await this.Get(id);

            bool possuiExecucoes = await context.Planejamentos
                                                .AnyAsync(pl => pl.IdPaciente == id && pl.Execucaos.Any());

            if (possuiExecucoes)
            {
                paciente!.Ativo = StatusAtivo.N.ToString();
                context.Pacientes.Update(paciente);

                var planejamentosPaciente = await context.Planejamentos
                                                         .Where(pl => pl.IdPaciente == id && pl.Ativo == StatusAtivo.S.ToString())
                                                         .ToListAsync();

                foreach (var planejamento in planejamentosPaciente)
                {
                    planejamento.Ativo = StatusAtivo.N.ToString();
                    planejamento.Status = Status.INTERROMPIDO.ToString();
                }

                context.Planejamentos.UpdateRange(planejamentosPaciente);
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

        /// <summary>
        /// Buscar um paciente na base de dados
        /// </summary>
        /// <param name="id">id do paciente</param>
        /// <returns>Dados do paciente</returns>
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

        /// <summary>
        /// Buscar todos os pacientes cadastrados
        /// </summary>
        /// <returns>Lista de paciente</returns>
        public async Task<IEnumerable<Paciente>> GetAll()
        {
            return await context.Pacientes
                                .AsNoTracking()
                                .Include(p => p.Planejamentos)
                                    .ThenInclude(pl => pl.Execucaos)
                                .ToListAsync();
        }

        /// <summary>
        /// Reativa um paciente com status inativo
        /// </summary>
        /// <param name="id">id do paciente</param>
        public async Task Activate(uint id)
        {
            var paciente = await this.Get(id);
            if (paciente!.Ativo == StatusAtivo.S.ToString())
            {
                return;
            }
            paciente.Ativo = StatusAtivo.S.ToString();
            context.Pacientes.Update(paciente);
            await context.SaveChangesAsync();
        }
    }
}