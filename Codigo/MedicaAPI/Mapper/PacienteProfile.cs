using AutoMapper;
using Core;
using Core.Dto.PacienteDto;


namespace MedicaAPI.Mapper
{
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            CreateMap<Paciente, PacienteMobileDto>()
                .ForMember(dest => dest.PossuiDeficiencia, 
                    opt => opt.MapFrom(src => src.Deficiencia.Any()));
        }
    }
}
