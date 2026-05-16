namespace Core.Service
{
    public interface IMedicamentoService
    {
        Medicamento? Get(uint id);

        uint Create(Medicamento medicamento);

        void Edit(Medicamento medicamento);

        void Delete(uint id);

        IEnumerable<Medicamento> GetAll();
    }
}
