using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class CuidadorService : ICuidadorService
    {

        private readonly MedicaContext context;

        public CuidadorService(MedicaContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Criar um novo cuidador na base de dados
        /// </summary>
        /// <param name="cuidador">Dados do cuidador</param>
        /// <returns>Id do novo cuidador</returns>
        public async Task<uint> Create(Cuidador cuidador)
        {
            await context.AddAsync(cuidador);
            await context.SaveChangesAsync();
            return cuidador.Id;
        }

        /// <summary>
        /// Buscar todos os cuidadores cadastrados
        /// </summary>
        /// <returns>Lista de cuidadores</returns>
        public async Task<IEnumerable<Cuidador>> GetAll()
        {
            return await context.Cuidadors
                .Include(c => c.Vinculos)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Busca o ID do cuidador baseado no seu CPF
        /// </summary>
        /// <returns>Id do novo cuidador</returns>
        public async Task<uint> GetIdByCpf(string cpf)
        {
            return await context.Cuidadors
                .AsNoTracking()
                .Where(c => c.Cpf == cpf)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }
    }
}
