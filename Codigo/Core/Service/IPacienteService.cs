namespace Core.Service
{
    public interface IPacienteService
    {
        Paciente? Get(uint id);

        uint Create(Paciente paciente);

        void Edit(Paciente paciente);

        void Delete(uint id);

        IEnumerable<Paciente> GetAll();
    }
}
