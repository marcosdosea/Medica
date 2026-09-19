using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoMapper;
using Core;
using Core.Dto.Planejamento;
using Core.Enum.Planejamento;
using MedicaWeb.Models;

namespace MedicaWeb.Mapper
{
    public class PlanejamentoProfile : Profile
    {
        public PlanejamentoProfile()
        {
            CreateMap<PlanejamentoViewModel, IEnumerable<Planejamento>>()
                .ConvertUsing((src, _, ctx) =>
                {
                    var itens = src.Itens != null && src.Itens.Any()
                        ? src.Itens
                        : [src];

                    foreach (var item in itens)
                    {
                        item.IdPaciente = src.IdPaciente;
                    }

                    return ctx.Mapper.Map<List<Planejamento>>(itens);
                });

            CreateMap<PlanejamentoViewModel, Planejamento>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src.Id))
                .ForMember(dest => dest.IdPaciente, opt => opt.MapFrom(src => src.IdPaciente))
                .ForMember(dest => dest.IdMedicamento, opt => opt.MapFrom(src => src.IdMedicamento))
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInicio))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.Continuo ? new DateTime(9999, 12, 31) : src.DataFim))
                .ForMember(dest => dest.Hora, opt => opt.MapFrom(src => src.Hora))
                .ForMember(dest => dest.IntervaloExecucao, opt => opt.MapFrom(src => src.IntervaloExecucao != default ? src.IntervaloExecucao : TimeSpan.FromHours(8)))
                .ForMember(dest => dest.Dosagem, opt => opt.MapFrom(src => src.Dosagem))
                .ForMember(dest => dest.UnidadeDosagem, opt => opt.MapFrom(src => src.Unidade.ToString()))
                .ForMember(dest => dest.DiaSemana, opt => opt.MapFrom(src => src.DiaSemana))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "NAO_INICIADO"))
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => "S"))
                .ReverseMap()
                .ForMember(dest => dest.Unidade, opt => opt.MapFrom(src => ParseEnum<UnidadeDosagem>(src.UnidadeDosagem)))
                .ForMember(dest => dest.Continuo, opt => opt.MapFrom(src => src.DataFim.Year > 9000));

            CreateMap<Planejamento, PlanejamentoDetailsDto>()
                .ForMember(dest => dest.NomePaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation.Nome))
                .ForMember(dest => dest.CpfPaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation.Cpf))
                .ForMember(dest => dest.NomeMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome))
                .ForMember(dest => dest.ApelidoMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Apelido))
                .ForMember(dest => dest.Hora, opt => opt.MapFrom(src => src.Hora))
                .ForMember(dest => dest.IntervaloExecucao, opt => opt.MapFrom(src => src.IntervaloExecucao))
                .ForMember(dest => dest.DiaSemana, opt => opt.MapFrom(src => src.DiaSemana))
                .ForMember(dest => dest.Unidade, opt => opt.MapFrom(src => ParseEnum<UnidadeDosagem>(src.UnidadeDosagem)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseEnum<Status>(src.Status)))
                .ForMember(dest => dest.Execucoes, opt => opt.MapFrom(src => src.Execucaos));

            CreateMap<Execucao, PlanejamentoDetailsDto.ExecucaoDto>()
                .ForMember(dest => dest.DataConfirmacao, opt => opt.MapFrom(src => src.DataConfirmacao.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.HoraConfirmacao, opt => opt.MapFrom(src => src.HoraConfirmacao.HasValue ? src.HoraConfirmacao.Value.ToString(@"hh\:mm") : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseEnum<Core.Enum.Execucao.Status>(src.Status)));

            CreateMap<Planejamento, PlanejamentoDto>()
                .ForMember(dest => dest.NomePaciente, opt => opt.MapFrom(src => src.IdPacienteNavigation.Nome))
                .ForMember(dest => dest.NomeMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome))
                .ForMember(dest => dest.ApelidoMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Apelido))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ObterStatusDisplay(src.Status)));

            CreateMap<PlanejamentoDto, Planejamento>();

            CreateMap<Planejamento, PlanejamentoItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdPaciente, opt => opt.MapFrom(src => src.IdPaciente))
                .ForMember(dest => dest.IdMedicamento, opt => opt.MapFrom(src => src.IdMedicamento))
                .ForMember(dest => dest.MedicamentoNome, opt => opt.MapFrom(src => src.IdMedicamentoNavigation.Nome))
                .ForMember(dest => dest.DataInicioFormatada, opt => opt.MapFrom(src => src.DataInicio.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.DataInicioIso, opt => opt.MapFrom(src => src.DataInicio.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.DataFimFormatada, opt => opt.MapFrom(src => src.DataFim.Year > 9000 ? "Contínuo" : src.DataFim.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.DataFimIso, opt => opt.MapFrom(src => src.DataFim.Year > 9000 ? "" : src.DataFim.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Continuo, opt => opt.MapFrom(src => src.DataFim.Year > 9000))
                .ForMember(dest => dest.Hora, opt => opt.MapFrom(src => src.Hora.ToString(@"hh\:mm")))
                .ForMember(dest => dest.IntervaloFormatado, opt => opt.MapFrom(src => src.IntervaloExecucao.ToString(@"hh\:mm")))
                .ForMember(dest => dest.DiaSemana, opt => opt.MapFrom(src => src.DiaSemana))
                .ForMember(dest => dest.Dosagem, opt => opt.MapFrom(src => $"{src.Dosagem} {src.UnidadeDosagem}"))
                .ForMember(dest => dest.DosagemValor, opt => opt.MapFrom(src => src.Dosagem))
                .ForMember(dest => dest.UnidadeDosagem, opt => opt.MapFrom(src => src.UnidadeDosagem));
        }

        private static TEnum ParseEnum<TEnum>(string? valor) where TEnum : struct, System.Enum =>
            Enum.TryParse<TEnum>(valor, true, out var resultado) ? resultado : default;

        private static string ObterStatusDisplay(string status) =>
            Enum.TryParse<Status>(status, true, out var e)
                ? typeof(Status).GetField(e.ToString())?.GetCustomAttribute<DisplayAttribute>()?.Name ?? status
                : status;
    }
}