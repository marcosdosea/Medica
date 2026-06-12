using Core;
using Core.Enum;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class MedicamentoService : IMedicamentoService
    {

        private readonly MedicaContext context;

        public MedicamentoService(MedicaContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Criar um novo medicamento na base de dados
        /// </summary>
        /// <param name="medicamento">Dados do medicamento</param>
        /// <returns>Id do novo medicamento</returns>
        public async Task<uint> Create(Medicamento medicamento)
        {
            await context.AddAsync(medicamento);
            await context.SaveChangesAsync();
            return medicamento.Id;
        }

        /// <summary>
        /// Remover dados de um medicamento da base de dados
        /// </summary>
        /// <param name="id">id do medicamento</param>
        public async Task Delete(uint id)
        {
            var medicamento = await this.Get(id);

            bool possuiExecucoes = await context.Planejamentos
                                                .AnyAsync(pl => pl.IdMedicamento == id && pl.Execucaos.Any());

            if (possuiExecucoes)
            {
                medicamento!.Ativo = StatusAtivo.N.ToString();
                context.Medicamentos.Update(medicamento);

                var planejamentosMedicamento = await context.Planejamentos
                                                            .Where(pl => pl.IdMedicamento == id && pl.Ativo == StatusAtivo.S.ToString())
                                                            .ToListAsync();

                foreach (var planejamento in planejamentosMedicamento)
                {
                    planejamento.Ativo = StatusAtivo.N.ToString();
                    planejamento.Status = Core.Enum.Planejamento.Status.INTERROMPIDO.ToString();
                }

                context.Planejamentos.UpdateRange(planejamentosMedicamento);
            }
            else
            {
                if (medicamento!.Alergia != null && medicamento.Alergia.Count != 0)
                {
                    context.Alergia.RemoveRange(medicamento.Alergia);
                }

                if (medicamento.Planejamentos != null && medicamento.Planejamentos.Count != 0)
                {
                    context.Planejamentos.RemoveRange(medicamento.Planejamentos);
                }

                context.Medicamentos.Remove(medicamento);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualizar dados de um medicamento da base de dados
        /// </summary>
        /// <param name="medicamento">Novos dados do medicamento</param>
        public async Task Edit(Medicamento medicamento)
        {
            context.Update(medicamento);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Buscar um medicamento na base de dados
        /// </summary>
        /// <param name="id">id do medicamento</param>
        /// <returns>Dados do medicamento</returns>
        public async Task<Medicamento?> Get(uint id)
        {
            return await context.Medicamentos
                                .Include(m => m.Alergia)
                                .Include(m => m.Planejamentos)
                                    .ThenInclude(pl => pl.Execucaos)
                                .FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Buscar todos os medicamentos cadastrados
        /// </summary>
        /// <returns>Lista de medicamentos</returns>
        public async Task<IEnumerable<Medicamento>> GetAll()
        {
            return await context.Medicamentos.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Reativa um medicamento com status inativo
        /// </summary>
        /// <param name="id">id do medicamento</param>
        public async Task Activate(uint id)
        {
            var medicamento = await this.Get(id);
            if (medicamento!.Ativo == StatusAtivo.S.ToString())
            {
                return;
            }
            medicamento.Ativo = StatusAtivo.S.ToString();
            context.Medicamentos.Update(medicamento);
            await context.SaveChangesAsync();
        }
    }
}
