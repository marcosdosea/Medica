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
        public uint Create(Medicamento medicamento)
        {
            context.Add(medicamento);
            context.SaveChanges();
            return medicamento.Id;
        }

        /// <summary>
        /// Remover dados de um medicamento da base de dados
        /// </summary>
        /// <param name="id">id do medicamento</param>
        public void Delete(uint id)
        {
            context.Remove(new Medicamento { Id = id });
            context.SaveChanges();
        }

        /// <summary>
        /// Atualizar dados de um medicamento da base de dados
        /// </summary>
        /// <param name="medicamento">Novos dados do medicamento</param>
        public void Edit(Medicamento medicamento)
        {
            context.Update(medicamento);
            context.SaveChanges();
        }

        /// <summary>
        /// Buscar um medicamento na base de dados
        /// </summary>
        /// <param name="id">id do medicamento</param>
        /// <returns>Dados do medicamento</returns>
        public Medicamento? Get(uint id)
        {
            return context.Medicamentos.AsNoTracking().FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Buscar todos os medicamentos cadastrados
        /// </summary>
        /// <returns>Lista de medicamentos</returns>
        public IEnumerable<Medicamento> GetAll()
        {
            return context.Medicamentos.AsNoTracking();
        }
    }
}
