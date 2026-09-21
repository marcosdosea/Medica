using AutoMapper;
using Core;
using Core.Dto.Execucao;

namespace MedicaAPI.Mapper
{
    public class ExecucaoProfile : Profile
    {
        public ExecucaoProfile()
        {
            CreateMap<ExecucaoRequestDto, Execucao>()
                .ForMember(dest => dest.DataConfirmacao,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.DataConfirmacao)
                        ? DateTime.Parse(src.DataConfirmacao)
                        : (DateTime?)null))
                .ForMember(dest => dest.HoraConfirmacao,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.HoraConfirmacao)
                        ? TimeSpan.Parse(src.HoraConfirmacao)
                        : (TimeSpan?)null));
        }
    }
}
