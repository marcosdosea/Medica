using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PacienteService : IPacienteService
    {

        private readonly MedicaContext context;

        public PacienteService(MedicaContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Criar um novo medicamento na base de dados
        /// </summary>
        /// <param name="medicamento">Dados do medicamento</param>
        /// <returns>Id do novo medicamento</returns>
        public uint Create(Paciente paciente)
        {
            context.Add(paciente);
            context.SaveChanges();
            return paciente.Id;
        }

        
        /// <summary>
        /// Remover dados de um medicamento da base de dados
        /// </summary>
        /// <param name="id">id do medicamento</param>
        public void Delete(uint id)
        {
            context.Remove(id);
            context.SaveChanges();
        }

        /// <summary>
        /// Atualizar dados de um medicamento da base de dados
        /// </summary>
        /// <param name="paciente">Novos dados do medicamento</param>
        public void Edit(Paciente paciente)
        {
            context.Update(paciente);
            context.SaveChanges();
        }

        /// <summary>
        /// Buscar um medicamento na base de dados
        /// </summary>
        /// <param name="id">id do medicamento</param>
        /// <returns>Dados do medicamento</returns>
        public Paciente? Get(uint id)
        {
            return context.Pacientes.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Buscar todos os medicamentos cadastrados
        /// </summary>
        /// <returns>Lista de medicamentos</returns>
        public IEnumerable<Paciente> GetAll()
        {
            return context.Pacientes.AsNoTracking();
        }
    }
}
