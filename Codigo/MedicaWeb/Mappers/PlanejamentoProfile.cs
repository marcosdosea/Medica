using AutoMapper;
using Core;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class PlanejamentoProfile : Profile
    {
        public PlanejamentoProfile()
        {
            CreateMap<PlanejamentoViewModel, Planejamento>().ReverseMap();
        }
    }   
}
