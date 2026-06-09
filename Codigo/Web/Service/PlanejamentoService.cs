using Core;
using Core.Enum;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PlanejamentoService : IPlanejamentoService
    {
        private readonly MedicaContext context;

        public PlanejamentoService(MedicaContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Criar um novo planejamento na base de dados
        /// </summary>
        /// <param name="planejamento">Dados do planejamento</param>
        /// <returns>Id do novo planejamento</returns>
        public async Task<uint> Create(Planejamento planejamento)
        {
            await context.Planejamentos.AddAsync(planejamento);
            await context.SaveChangesAsync();
            return (uint)planejamento.Id;
        }

        /// <summary>
        /// Remover dados de um planejamento da base de dados
        /// </summary>
        /// <param name="id">id do planejamento</param>
        public async Task Delete(uint id)
        {
            var planejamento = await context.Planejamentos.FindAsync((int) id);

            bool possuiExecucao = await context.Execucaos.AnyAsync(e => e.IdPlanejamento == id);

            if (possuiExecucao)
            {
                planejamento!.Ativo = StatusAtivo.N.ToString();
                context.Planejamentos.Update(planejamento);
            }
            else
            {
                context.Planejamentos.Remove(planejamento!);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Buscar um planejamento na base de dados
        /// </summary>
        /// <param name="id">id do planejamento</param>
        /// <returns>Dados do planejamento</returns>
        public async Task Edit(Planejamento planejamento)
        {
            context.Planejamentos.Update(planejamento);
            await context.SaveChangesAsync();
        }

        public async Task<Planejamento?> Get(uint id)
        {
            return await context.Planejamentos.FindAsync((int) id);
        }

        /// <summary>
        /// Buscar todos os planejamentos cadastrados
        /// </summary>
        /// <returns>Lista de planejamentos</returns>
        public async Task<IEnumerable<Planejamento>> GetAll()
        {
            return await context.Planejamentos
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                .Where(p => p.IdPacienteNavigation.Ativo == StatusAtivo.S.ToString() &&
                    p.IdMedicamentoNavigation.Ativo == StatusAtivo.S.ToString())
                .ToListAsync();
        }

        /// <summary>
        /// Reativa um planejamento com status inativo
        /// </summary>
        /// <param name="id">id do planejamento</param>
        public async Task Activate(uint id)
        {
            var planejamento = await this.Get(id);
            if (planejamento!.Ativo == StatusAtivo.S.ToString())
            {
                return;
            }
            planejamento.Ativo = StatusAtivo.S.ToString();
            context.Planejamentos.Update(planejamento);
            await context.SaveChangesAsync();
        }
    }
}
