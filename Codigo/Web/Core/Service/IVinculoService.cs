namespace Core.Service
{
    public interface IVinculoService
    {
        Task Create(Vinculo vinculo);

        Task DeleteByPaciente(uint idPaciente);
    }
}