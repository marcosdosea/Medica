using AutoMapper;
using Core;
using MedicaWeb.Models;

namespace MedicaWeb.Mappers
{
    public class MedicamentoProfile : Profile
    {
        public MedicamentoProfile() {
            CreateMap<Medicamento, MedicamentoViewModel>().ReverseMap();
        }
    }
}
