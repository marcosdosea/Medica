using AutoMapper;
using Core;
using Core.Dto.Dispositivo;

namespace MedicaWeb.Mapper
{
    public class DispositivoProfile : Profile
    {
        public DispositivoProfile()
        {
            CreateMap<Dispositivopaciente, DispositivoDto>()
                .ForMember(dest => dest.IdPaciente, opt => opt.MapFrom(src => src.IdPaciente))
                .ForMember(dest => dest.NomePaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation != null ? src.IdPacienteNavigation.Nome : string.Empty))
                .ForMember(dest => dest.QuantidadeDispositivos, opt => opt.MapFrom(src =>
                    src.IdPacienteNavigation != null && src.IdPacienteNavigation.Dispositivopacientes != null
                        ? src.IdPacienteNavigation.Dispositivopacientes.Count
                        : 0));
        }
    }
}
