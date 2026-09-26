using Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IEstoqueService
    {
        Task<int> Create(Estoque estoque, IEnumerable<uint> idsPacientes);
        Task Edit(Estoque estoque, IEnumerable<uint> idsPacientes);
        Task<IEnumerable<Estoque>> GetAllByCuidador(uint idCuidador);
        Task<Estoque?> Get(int id);
        Task Delete(int id);
    }
}
