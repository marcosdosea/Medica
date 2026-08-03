using Core;

namespace Core.Service
{
    public interface IExecucaoService
    {
        Task<uint> Create(Execucao execucao);
        Task<Execucao?> Get(uint id);
    }
}
