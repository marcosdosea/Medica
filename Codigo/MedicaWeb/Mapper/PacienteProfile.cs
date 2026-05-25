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
            CreateMap<PacienteDetailsDto, Paciente>();

            CreateMap<Paciente, PacienteDto>();

            CreateMap<Paciente, PacienteDetailsDto>()
            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo.ToString()))
            .ForMember(dest => dest.Escolaridade, opt => opt.MapFrom(src => src.Escolaridade.ToString()));
        }
    }
}
