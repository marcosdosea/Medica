using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class VinculoService : IVinculoService
    {
        private readonly MedicaContext context;

        public VinculoService(MedicaContext context)
        {
            this.context = context;
        }

        public async Task Create(Vinculo vinculo)
        {
            await context.Vinculos.AddAsync(vinculo);
            await context.SaveChangesAsync();
        }

        public async Task DeleteByPaciente(uint idPaciente)
        {
            var vinculos = await context.Vinculos
                .Where(v => v.IdPaciente == idPaciente)
                .ToListAsync();

            if (vinculos.Any())
            {
                context.Vinculos.RemoveRange(vinculos);
                await context.SaveChangesAsync();
            }
        }
    }
}