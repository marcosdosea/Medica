namespace Core.Service
{
    public interface IPacienteService
    {
        IEnumerable<Paciente> GetByMedicamento(uint idMedicamento);

        IEnumerable<Paciente> GetAll();
    }
}
