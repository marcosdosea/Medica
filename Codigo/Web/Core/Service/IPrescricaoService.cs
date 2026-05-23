namespace Core.Service
{
    public interface IPrescricaoService
    {
        void Create(Prescricao medicamento);

        void DeleteByMedicamento(uint idMedicamento);

        IEnumerable<Prescricao> GetAll(uint idMedicamento);
    }
}
