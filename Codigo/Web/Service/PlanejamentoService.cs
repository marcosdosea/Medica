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

        public async Task<uint> Create(Planejamento planejamento)
        {
            await context.Planejamentos.AddAsync(planejamento);
            await context.SaveChangesAsync();
            return (uint)planejamento.Id;
        }

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

        public async Task Edit(Planejamento planejamento)
        {
            context.Planejamentos.Update(planejamento);
            await context.SaveChangesAsync();
        }

        public async Task<Planejamento?> Get(uint id)
        {
            return await context.Planejamentos.FindAsync((int) id);
        }

        public async Task<IEnumerable<Planejamento>> GetAll()
        {
            return await context.Planejamentos
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                .ToListAsync();
        }
    }
}
