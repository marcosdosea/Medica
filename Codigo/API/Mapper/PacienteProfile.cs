using AutoMapper;
using Core;
using Core.Dto.Paciente;

namespace MedicaAPI.Mapper
{
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            CreateMap<Paciente, PacienteMobileDto>()
                .ForMember(dest => dest.PossuiDeficiencia,
                    opt => opt.MapFrom(src => src.PossuiDeficiencia == 1 || !string.IsNullOrWhiteSpace(src.Deficiencia)))
                .ForMember(dest => dest.Deficiencias,
                    opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.Deficiencia)
                        ? new List<PacienteMobileDto.DeficienciaMobileDto> { new() { Descricao = src.Deficiencia } }
                        : new List<PacienteMobileDto.DeficienciaMobileDto>()));
        }
    }
}
