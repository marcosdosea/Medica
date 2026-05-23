using AutoMapper;
using Core;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class DeficienciumProfile : Profile
    {
        public DeficienciumProfile()
        {
            CreateMap<DeficienciumViewModel, Deficiencium>().ReverseMap();
        }
    }
}
