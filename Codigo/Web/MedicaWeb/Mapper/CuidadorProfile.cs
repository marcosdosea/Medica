using AutoMapper;
using Core;
using Core.Dto.Cuidador;
using Util;

namespace MedicaWeb.Mapper
{
    public class CuidadorProfile : Profile
    {
        public CuidadorProfile()
        {
            CreateMap<Cuidador, CuidadorDto>()
                .ForMember(dest => dest.QuantidadePacientes, opt => opt.MapFrom(src => src.Vinculos.Count))
                .AfterMap((src, dest) =>
                {
                    dest.Cpf = FormatterCpf.FormatarCpf(src.Cpf);
                });
        }
    }
}