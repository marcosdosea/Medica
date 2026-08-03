using AutoMapper;
using Core;
using Core.Dto.Execucao;

namespace MedicaAPI.Mapper
{
    public class ExecucaoProfile : Profile
    {
        public ExecucaoProfile()
        {
            // Mapeia o DTO recebido pelo controller (string → tipos corretos)
            CreateMap<ExecucaoRequestDto, Execucao>()
                .ForMember(dest => dest.DataConfirmacao, opt =>
                    opt.MapFrom(src => DateTime.Parse(src.DataConfirmacao)))
                .ForMember(dest => dest.HoraConfirmacao, opt =>
                    opt.MapFrom(src => string.IsNullOrWhiteSpace(src.HoraConfirmacao)
                        ? (TimeSpan?)null
                        : TimeSpan.Parse(src.HoraConfirmacao)));
                
        }
    }
}
