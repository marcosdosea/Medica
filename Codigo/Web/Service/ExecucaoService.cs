using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ExecucaoService : IExecucaoService
    {
        private readonly MedicaContext context;

        public ExecucaoService(MedicaContext context)
        {
            this.context = context;
        }

        public async Task<uint> Create(Execucao execucao)
        {
            if (execucao.HoraConfirmacao == null)
            {
                execucao.Status = "FALHA"; 
            }
            else
            {
                execucao.Status = "SUCESSO";

                var planejamento = await context.Planejamentos
                    .Include(p => p.IdMedicamentoNavigation)
                    .FirstOrDefaultAsync(p => p.Id == execucao.IdPlanejamento);

                if (planejamento != null && planejamento.IdMedicamentoNavigation != null)
                {
                    planejamento.IdMedicamentoNavigation.Quantidade -= planejamento.Dosagem;

                    if (planejamento.IdMedicamentoNavigation.Quantidade < 0)
                    {
                        planejamento.IdMedicamentoNavigation.Quantidade = 0;
                    }

                    context.Medicamentos.Update(planejamento.IdMedicamentoNavigation);
                }
            }

            await context.Execucaos.AddAsync(execucao);

            await context.SaveChangesAsync();

            return execucao.Id;
        }

        public async Task<Execucao?> Get(uint id)
        {
            return await context.Execucaos
                .Include(e => e.IdPlanejamentoNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}