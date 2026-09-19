using AutoMapper;
using Core;
using MedicaWeb.Models;
using Util;

namespace MedicaWeb.Mapper
{
    public class MedicamentoProfile : Profile
    {
        public MedicamentoProfile()
        {
            CreateMap<Medicamento, MedicamentoViewModel>()
                .ForMember(dest => dest.FotoMedicamento, opt => opt.Ignore());

            CreateMap<MedicamentoViewModel, Medicamento>()
                .ForMember(dest => dest.Foto, opt => opt.MapFrom((src, dest) =>
                    src.RemoverFoto ? null : (src.FotoMedicamento != null ? FotoHelper.ConverterFoto(src.FotoMedicamento) : dest.Foto)));
        }
    }
}
