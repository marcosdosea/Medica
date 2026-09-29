using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class EstoqueService : IEstoqueService
    {
        private readonly MedicaContext context;

        public EstoqueService(MedicaContext context)
        {
            this.context = context;
        }

        public async Task<int> Create(Estoque estoque)
        {
            var pacienteExiste = await context.Pacientes.AnyAsync(p => p.Id == estoque.IdPaciente);
            if (!pacienteExiste)
            {
                throw new ServiceException("Selecione um paciente válido para associar ao estoque.");
            }

            if (estoque.Quantidade == 0)
            {
                estoque.Status = "INSUFICIENTE";
            }
            else if (estoque.Quantidade <= estoque.QuantidadeMinima)
            {
                estoque.Status = "BAIXO";
            }
            else
            {
                estoque.Status = "REGULAR";
            }

            await context.Estoques.AddAsync(estoque);
            await context.SaveChangesAsync();
            return estoque.Id;
        }

        public async Task Edit(Estoque estoque)
        {
            var existing = await context.Estoques
                .FirstOrDefaultAsync(e => e.Id == estoque.Id);

            if (existing == null)
            {
                throw new ServiceException("Estoque não encontrado.");
            }

            var pacienteExiste = await context.Pacientes.AnyAsync(p => p.Id == estoque.IdPaciente);
            if (!pacienteExiste)
            {
                throw new ServiceException("Selecione um paciente válido para associar ao estoque.");
            }

            existing.IdMedicamento = estoque.IdMedicamento;
            existing.IdPaciente = estoque.IdPaciente;
            existing.Quantidade = estoque.Quantidade;
            existing.QuantidadeMinima = estoque.QuantidadeMinima;
            existing.DataValidade = estoque.DataValidade;

            if (existing.Quantidade <= 0)
            {
                existing.Status = "INSUFICIENTE";
            }
            else if (existing.Quantidade <= existing.QuantidadeMinima)
            {
                existing.Status = "BAIXO";
            }
            else
            {
                existing.Status = "REGULAR";
            }

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Estoque>> GetAllByCuidador(uint idCuidador)
        {
            var idsPacientes = await context.Vinculos
                .Where(v => v.IdCuidador == idCuidador)
                .Select(v => v.IdPaciente)
                .ToListAsync();

            return await context.Estoques
                .AsNoTracking()
                .Include(e => e.IdMedicamentoNavigation)
                .Include(e => e.IdPacienteNavigation)
                .Where(e => idsPacientes.Contains(e.IdPaciente))
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<Estoque?> Get(int id)
        {
            return await context.Estoques
                .Include(e => e.IdMedicamentoNavigation)
                .Include(e => e.IdPacienteNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task Delete(int id)
        {
            var estoque = await context.Estoques
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estoque != null)
            {
                context.Estoques.Remove(estoque);
                await context.SaveChangesAsync();
            }
        }
    }
}
