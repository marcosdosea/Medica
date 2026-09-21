using AutoMapper;
using Core;
using Core.Dto.Planejamento;

namespace MedicaAPI.Mapper
{
    public class PlanejamentoProfile : Profile
    {
        public PlanejamentoProfile()
        {
            CreateMap<Planejamento, PlanejamentoMobileResponseDto>();

            CreateMap<Planejamento, PlanejamentoMobileDetailsDto>()
                .ForMember(dest => dest.NomeMedicamento,
                    opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome))
                .ForMember(dest => dest.ApelidoMedicamento,
                    opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Apelido))
                .ForMember(dest => dest.FotoMedicamento,
                    opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Foto != null && src.IdMedicamentoNavigation.Foto.Length > 0
                        ? Convert.ToBase64String(src.IdMedicamentoNavigation.Foto)
                        : null));
        }
    }
}