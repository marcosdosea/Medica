using AutoMapper;
using Core;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class PacienteProfile: Profile
    {
        public PacienteProfile()
        {
            CreateMap<PacienteViewModel, Paciente>();
        }
    }
}
