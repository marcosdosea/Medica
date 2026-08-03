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
                // sbyte 1 = possui deficiência, ou possui itens na lista
                .ForMember(dest => dest.PossuiDeficiencia,
                    opt => opt.MapFrom(src => src.PossuiDeficiencia == 1 || (src.Deficiencia != null && src.Deficiencia.Any())))
                // Sexo e Escolaridade têm o mesmo nome na entidade e no DTO — AutoMapper mapeia automaticamente
                .ForMember(dest => dest.Deficiencias,
                    opt => opt.MapFrom(src => src.Deficiencia));

            CreateMap<Deficiencium, PacienteMobileDto.DeficienciaMobileDto>();
        }
    }
}
