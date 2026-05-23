using AutoMapper;
using Core;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class AlergiumProfile : Profile
    {
        public AlergiumProfile()
        {
            CreateMap<AlergiumViewModel, Alergium>().ReverseMap();
        }
    }
}
