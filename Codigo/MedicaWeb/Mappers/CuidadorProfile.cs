using AutoMapper;
using Core;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class CuidadorProfile : Profile 
    {
        public CuidadorProfile()
        {
            CreateMap<Cuidador, CuidadorViewModel>().ReverseMap();
        }
    }
}
