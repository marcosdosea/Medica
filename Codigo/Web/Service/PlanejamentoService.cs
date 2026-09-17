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
        /// Criar uma lista de novos planejamentos na base de dados em lote
        /// </summary>
        /// <param name="planejamentos">Coleção de planejamentos a serem persistidos</param>
        /// <returns>True se gravou com sucesso</returns>
        public async Task<bool> Create(IEnumerable<Planejamento> planejamentos)
        {
            if (planejamentos == null || !planejamentos.Any())
                return false;

            await context.Planejamentos.AddRangeAsync(planejamentos);
            await context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Remover dados de um planejamento da base de dados
        /// </summary>
        /// <param name="id">id do planejamento</param>
        public async Task Delete(uint id)
        {
            var planejamento = await context.Planejamentos.FindAsync((int)id);

            if (planejamento == null)
            {
                throw new ServiceException("Planejamento não encontrado.");
            }

            bool possuiExecucao = await context.Execucaos.AnyAsync(e => e.IdPlanejamento == id);
            if (possuiExecucao)
            {
                planejamento.Ativo = StatusAtivo.N.ToString();
                planejamento.Status = Core.Enum.Planejamento.Status.INTERROMPIDO.ToString();
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
            bool possuiExecucao = await context.Execucaos
                .AnyAsync(e => e.IdPlanejamento == planejamento.Id);

            if (possuiExecucao)
            {
                throw new ServiceException("Não é possível editar um planejamento que possui execuções.");
            }

            context.Planejamentos.Update(planejamento);
            await context.SaveChangesAsync();
        }

        public async Task<Planejamento?> Get(uint id)
        {
            return await context.Planejamentos
                .AsNoTracking()
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                .Include(p => p.Execucaos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Buscar todos os planejamentos cadastrados (com filtro opcional por paciente)
        /// </summary>
        /// <returns>Lista de planejamentos</returns>
        public async Task<IEnumerable<Planejamento>> GetAll(uint idCuidador, uint? idPaciente = null)
        {
            var query = context.Planejamentos
                .AsNoTracking()
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                .Where(p => p.IdMedicamentoNavigation.IdCuidador == idCuidador);

            if (idPaciente.HasValue)
                query = query.Where(p => p.IdPaciente == idPaciente.Value);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Reativa um planejamento com status inativo
        /// </summary>
        /// <param name="id">id do planejamento</param>
        public async Task Activate(uint id)
        {
            var planejamento = await this.Get(id);

            if (planejamento == null)
            {
                throw new ServiceException("Planejamento não encontrado.");
            }

            if (planejamento.Ativo == StatusAtivo.S.ToString())
            {
                return;
            }

            var agora = DateTime.Now;
            bool isContinuo = planejamento.DataFim.Date == DateTime.MaxValue.Date;
            if (!isContinuo && planejamento.DataFim < agora)
            {
                throw new ServiceException("Não é possível reativar um planejamento com período encerrado.");
            }

            planejamento.Ativo = StatusAtivo.S.ToString();
            planejamento.Status = (planejamento.DataInicio > agora
                ? Core.Enum.Planejamento.Status.NAO_INICIADO
                : Core.Enum.Planejamento.Status.EM_ANDAMENTO).ToString();
            context.Planejamentos.Update(planejamento);
            await context.SaveChangesAsync();
        }
    }
}
