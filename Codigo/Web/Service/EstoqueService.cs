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

        public async Task<int> Create(Estoque estoque, IEnumerable<uint> idsPacientes)
        {
            var ids = idsPacientes?.ToList() ?? new List<uint>();
            if (!ids.Any())
            {
                throw new ServiceException("Selecione ao menos um paciente para associar ao estoque.");
            }

            var pacientes = await context.Pacientes
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (!pacientes.Any())
            {
                throw new ServiceException("Nenhum paciente válido encontrado.");
            }

            estoque.IdPacientes = pacientes;

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

        public async Task Edit(Estoque estoque, IEnumerable<uint> idsPacientes)
        {
            var existing = await context.Estoques
                .Include(e => e.IdPacientes)
                .FirstOrDefaultAsync(e => e.Id == estoque.Id);

            if (existing == null)
            {
                throw new ServiceException("Estoque não encontrado.");
            }

            var ids = idsPacientes?.ToList() ?? new List<uint>();
            if (!ids.Any())
            {
                throw new ServiceException("Selecione ao menos um paciente para associar ao estoque.");
            }

            var pacientes = await context.Pacientes
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (!pacientes.Any())
            {
                throw new ServiceException("Nenhum paciente válido encontrado.");
            }

            existing.IdMedicamento = estoque.IdMedicamento;
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

            existing.IdPacientes.Clear();
            foreach (var p in pacientes)
            {
                existing.IdPacientes.Add(p);
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
                .Include(e => e.IdPacientes)
                .Where(e => e.IdPacientes.Any(p => idsPacientes.Contains(p.Id)))
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<Estoque?> Get(int id)
        {
            return await context.Estoques
                .Include(e => e.IdMedicamentoNavigation)
                .Include(e => e.IdPacientes)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task Delete(int id)
        {
            var estoque = await context.Estoques
                .Include(e => e.IdPacientes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estoque != null)
            {
                estoque.IdPacientes.Clear();
                context.Estoques.Remove(estoque);
                await context.SaveChangesAsync();
            }
        }
    }
}
