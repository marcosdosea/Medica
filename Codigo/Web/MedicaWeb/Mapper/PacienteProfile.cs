using AutoMapper;
using Core;
using Core.Dto;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class PacienteProfile: Profile
    {
        public PacienteProfile()
        {
            CreateMap<PacienteViewModel, Paciente>();
            CreateMap<Paciente, PacienteDto>().ReverseMap();
        }
    }
}
