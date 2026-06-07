using AutoMapper;
using Core;
using Core.Dto.PacienteDto;
using static Core.Dto.PacienteDto.PacienteDetailsDto;

namespace MedicaWeb.Mapper
{
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            CreateMap<PacienteDetailsDto, Paciente>();

            CreateMap<Paciente, PacienteDto>()
                            .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo.ToString()));

            CreateMap<Paciente, PacienteDetailsDto>()
                            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo.ToString()))
                            .ForMember(dest => dest.Escolaridade, opt => opt.MapFrom(src => src.Escolaridade.ToString()))
                            .ForMember(dest => dest.Alergias, opt => opt.MapFrom(src => src.Alergia))
                            .ForMember(dest => dest.Deficiencias, opt => opt.MapFrom(src => src.Deficiencia));

            CreateMap<Deficiencium, PacienteDeficienciaDto>();

            CreateMap<Alergium, PacienteAlergiaDto>()
                .ForMember(dest => dest.MedicamentoNome,
                           opt => opt.MapFrom(src => src.IdMedicamentoNavigation != null ? src.IdMedicamentoNavigation.Nome : null));

            CreateMap<Vinculo, VinculoDto>()
                            .ForMember(dest => dest.Parentesco, opt => opt.MapFrom(src => src.Parentesco.ToString()));
        }
    }
}
