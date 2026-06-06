using Core;
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
            context.Remove(new Medicamento { Id = id });
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
            return await context.Medicamentos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Buscar todos os medicamentos cadastrados
        /// </summary>
        /// <returns>Lista de medicamentos</returns>
        public async Task<IEnumerable<Medicamento>> GetAll()
        {
            return await context.Medicamentos.AsNoTracking().ToListAsync();
        }
    }
}
