using System.Linq;
using AutoMapper;
using Core;
using Core.Dto.Estoque;
using MedicaWeb.Models;
using Util;

namespace MedicaWeb.Mapper
{
    public class EstoqueProfile : Profile
    {
        public EstoqueProfile()
        {
            CreateMap<Estoque, EstoqueItemDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdMedicamento, opt => opt.MapFrom(src => src.IdMedicamento))
                .ForMember(dest => dest.NomeMedicamento, opt => opt.MapFrom(src => src.IdMedicamentoNavigation != null ? src.IdMedicamentoNavigation.Nome : string.Empty))
                .ForMember(dest => dest.FormaFarmaceutica, opt => opt.MapFrom(src => src.IdMedicamentoNavigation != null ? src.IdMedicamentoNavigation.FormaFarmaceutica : string.Empty))
                .ForMember(dest => dest.IdsPacientes, opt => opt.MapFrom(src => src.IdPacientes.Select(p => p.Id).ToList()))
                .ForMember(dest => dest.PacientesNomes, opt => opt.MapFrom(src => string.Join(", ", src.IdPacientes.Select(p => PacienteHelper.FormatarPrimeiroEUltimoNome(p.Nome)))))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade))
                .ForMember(dest => dest.QuantidadeMinima, opt => opt.MapFrom(src => src.QuantidadeMinima))
                .ForMember(dest => dest.DataValidade, opt => opt.MapFrom(src => src.DataValidade))
                .ForMember(dest => dest.DataValidadeFormatada, opt => opt.MapFrom(src => src.DataValidade.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.DataValidadeIso, opt => opt.MapFrom(src => src.DataValidade.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<GerenciarEstoqueViewModel, Estoque>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdMedicamento, opt => opt.MapFrom(src => src.IdMedicamento))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade ?? 0))
                .ForMember(dest => dest.QuantidadeMinima, opt => opt.MapFrom(src => src.QuantidadeMinima ?? 0))
                .ForMember(dest => dest.DataValidade, opt => opt.MapFrom(src => src.DataValidade ?? System.DateTime.Today))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "REGULAR"));

            CreateMap<IEnumerable<Estoque>, IEnumerable<MedicamentoEstoqueDto>>()
                .ConvertUsing((src, _, ctx) => src
                    .Where(e => e.Quantidade > 0)
                    .SelectMany(e => e.IdPacientes.Select(p => new MedicamentoEstoqueDto
                    {
                        IdPaciente = p.Id,
                        IdMedicamento = e.IdMedicamento,
                        NomeMedicamento = e.IdMedicamentoNavigation != null ? e.IdMedicamentoNavigation.Nome : string.Empty,
                        FormaFarmaceutica = e.IdMedicamentoNavigation != null ? e.IdMedicamentoNavigation.FormaFarmaceutica : string.Empty
                    }))
                    .GroupBy(x => new { x.IdPaciente, x.IdMedicamento, x.NomeMedicamento, x.FormaFarmaceutica })
                    .Select(g => g.First())
                    .OrderBy(x => x.NomeMedicamento)
                    .ToList());

            CreateMap<List<Estoque>, IEnumerable<MedicamentoEstoqueDto>>()
                .ConvertUsing((src, _, ctx) => ctx.Mapper.Map<IEnumerable<MedicamentoEstoqueDto>>((IEnumerable<Estoque>)src));
        }
    }
}
