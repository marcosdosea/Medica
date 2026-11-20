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

        public async Task<bool> Create(IEnumerable<Planejamento> planejamentos)
        {
            if (planejamentos == null || !planejamentos.Any())
                return false;

            await context.Planejamentos.AddRangeAsync(planejamentos);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task Delete(uint id)
        {
            var planejamento = await context.Planejamentos.FindAsync((int)id)
                ?? throw new ServiceException("Planejamento não encontrado.");

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

        public async Task<IEnumerable<Planejamento>> GetAll(uint idCuidador)
        {
            return await context.Planejamentos
                .AsNoTracking()
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                .Where(p => p.IdMedicamentoNavigation.IdCuidador == idCuidador)
                .ToListAsync();
        }

        public async Task<IEnumerable<Planejamento>> GetAllByPaciente(uint idPaciente, DateTime? ultimaSincronizacao = null)
        {
            var query = context.Planejamentos
                .AsNoTracking()
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                .Where(p => p.IdPaciente == idPaciente && p.Ativo == StatusAtivo.S.ToString());

            if (ultimaSincronizacao.HasValue)
            {
                query = query.Where(p => p.DataAtualizacao >= ultimaSincronizacao.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<int>> GetIdsExcluidosByPaciente(uint idPaciente, DateTime? ultimaSincronizacao = null)
        {
            if (!ultimaSincronizacao.HasValue)
            {
                return [];
            }

            return await context.Planejamentos
                .AsNoTracking()
                .Where(p => p.IdPaciente == idPaciente
                         && p.Ativo == StatusAtivo.N.ToString()
                         && p.DataAtualizacao >= ultimaSincronizacao.Value)
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task Activate(uint id)
        {
            var planejamento = await this.Get(id)
                ?? throw new ServiceException("Planejamento não encontrado.");

            if (planejamento.Ativo == StatusAtivo.S.ToString())
            {
                return;
            }

            var agora = DateTime.Now;
            bool isContinuo = planejamento.DataFim == null;
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
