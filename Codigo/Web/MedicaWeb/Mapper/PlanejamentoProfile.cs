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
                            .ForMember(dest => dest.NomeMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome))
                            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Planejamento, PlanejamentoDetailsDto>()
                .ForMember(dest => dest.NomePaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation.Nome))
                .ForMember(dest => dest.CpfPaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation.Cpf))
                .ForMember(dest => dest.NomeMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome))
                .ForMember(dest => dest.ApelidoMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Apelido))
                .ForMember(dest => dest.Execucoes, opt => opt.MapFrom(src => src.Execucaos.Select(e => new PlanejamentoDetailsDto.ExecucaoDto
                {
                    Id = e.Id,
                    DataConfirmacao = e.DataConfirmacao.ToString("dd/MM/yyyy"),
                    HoraConfirmacao = e.HoraConfirmacao.HasValue ? e.HoraConfirmacao.Value.ToString(@"hh\:mm") : null,
                    Status = (Core.Enum.Execucao.Status)Enum.Parse(typeof(Core.Enum.Execucao.Status), e.Status)
                })))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (Core.Enum.Planejamento.Status)Enum.Parse(typeof(Core.Enum.Planejamento.Status), src.Status)));
        }
    }
}
