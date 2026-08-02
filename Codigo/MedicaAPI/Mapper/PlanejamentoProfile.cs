using AutoMapper;
using Core;
using Core.Dto.Planejamento;

namespace MedicaAPI.Mapper
{
    public class PlanejamentoProfile : Profile
    {
        public PlanejamentoProfile()
        {
            CreateMap<Planejamento, PlanejamentoMobileDto>()
                .ForMember(dest => dest.Horario,
                    opt => opt.MapFrom(src => src.Hora.ToString(@"hh\:mm")))
                .ForMember(dest => dest.UnidadeDosagem,
                    opt => opt.MapFrom(src => src.UnidadeDosagem.ToString()))
                // InstrucaoConsumo não existe na entidade — ignorado para evitar NullReferenceException
                .ForMember(dest => dest.InstrucaoConsumo,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Medicamento,
                    opt => opt.MapFrom(src => src.IdMedicamentoNavigation));

            CreateMap<Medicamento, MedicamentoMobileDto>()
                .ForMember(dest => dest.FormaFarmaceutica,
                    opt => opt.MapFrom(src => src.FormaFarmaceutica.ToString()))
                .ForMember(dest => dest.Foto,
                    opt => opt.MapFrom(src => src.Foto != null ? Convert.ToBase64String(src.Foto) : string.Empty));
        }
    }
}