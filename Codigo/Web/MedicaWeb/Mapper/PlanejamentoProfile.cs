using AutoMapper;
using Core;
using Core.Dto.Planejamento;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class PlanejamentoProfile : Profile
    {
        public PlanejamentoProfile()
        {
            CreateMap<PlanejamentoViewModel, Planejamento>().ReverseMap();

            CreateMap<Planejamento, PlanejamentoDto>()
                            .ForMember(dest => dest.NomePaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation.Nome))
                            .ForMember(dest => dest.NomeMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome));
        }
    }   
}
