using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PrescricaoService : IPrescricaoService
    {
        private readonly MedicaContext context;

        public PrescricaoService(MedicaContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Criar uma nova prescrição na base de dados
        /// </summary>
        /// <param name="medicamento">Dados da prescrição</param>
        public void Create(Prescricao prescricao)
        {
            context.Add(prescricao);
            context.SaveChanges();
        }

        /// <summary>
        /// Remove todos os vínculos de pacientes de um determinado medicamento
        /// </summary>
        /// <param name="idMedicamento">Id do medicamento</param>
        public void DeleteByMedicamento(uint idMedicamento)
        {
            var prescricoes = context.Prescricaos
                             .Where(p => p.IdMedicamento == idMedicamento);
            context.Prescricaos.RemoveRange(prescricoes);
            context.SaveChanges();
        }

        /// <summary>
        /// Buscar todas as prescrições cadastradas
        /// </summary>
        /// <returns>Lista de prescrições</returns>
        public IEnumerable<Prescricao> GetAll(uint idMedicamento)
        {
            return context.Prescricaos
                    .Include(p => p.IdPacienteNavigation)
                    .Where(p => p.IdMedicamento == idMedicamento)
                    .ToList();
        }
    }
}
