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
        /// Buscar todos os pacientes cadastrados
        /// </summary>
        /// <returns>Lista de pacientes</returns>
        public IEnumerable<Paciente> GetAll()
        {
            return context.Pacientes
                    .AsNoTracking()
                    .OrderBy(p => p.Nome)
                    .ToList();
        }

        /// <summary>
        /// Buscar um paciente específico pelo ID
        /// </summary>
        /// <param name="id">ID do paciente</param>
        /// <returns>Dados do paciente ou null se não encontrado</returns>
        public Paciente? Get(uint id)
        {
            return context.Pacientes
                    .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Buscar todos os pacientes cadastrados por medicamento
        /// </summary>
        /// <param name="idMedicamento">ID do medicamento</param>
        /// <returns>Lista de pacientes vinculados ao medicamento</returns>
        public IEnumerable<Paciente> GetByMedicamento(uint idMedicamento)
        {
            return context.Prescricaos
                    .AsNoTracking()
                    .Where(p => p.IdMedicamento == idMedicamento)
                    .Select(p => p.IdPacienteNavigation)
                    .Distinct()
                    .OrderBy(p => p.Nome)
                    .ToList();
        }

        /// <summary>
        /// Cadastrar um novo paciente no sistema
        /// </summary>
        /// <param name="paciente">Instância com os dados do paciente</param>
        /// <returns>O ID do paciente gerado pelo banco</returns>
        public uint Create(Paciente paciente)
        {
            context.Add(paciente);
            context.SaveChanges();
            return paciente.Id;
        }

        /// <summary>
        /// Editar os dados de um paciente existente
        /// </summary>
        /// <param name="paciente">Instância com os dados atualizados do paciente</param>
        public void Edit(Paciente paciente)
        {
            context.Update(paciente);
            context.SaveChanges();
        }

        /// <summary>
        /// Remover um paciente do sistema pelo ID
        /// </summary>
        /// <param name="id">ID do paciente a ser removido</param>
        public void Delete(uint id)
        {
            var paciente = context.Pacientes.Find(id);
            if (paciente != null)
            {
                context.Remove(paciente);
                context.SaveChanges();
            }
        }
    }
}