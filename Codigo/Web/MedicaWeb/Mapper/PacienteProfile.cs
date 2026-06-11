using AutoMapper;
using Core;
using Core.Dto.Paciente;
using static Core.Dto.Paciente.PacienteDetailsDto;

namespace MedicaWeb.Mapper
{
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            CreateMap<PacienteDetailsDto, Paciente>();

            CreateMap<Paciente, PacienteDto>()
                            .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo.ToString()))
                            .ForMember(dest => dest.ExecucoesFalhas, opt => opt.MapFrom(src =>
                                src.Planejamentos
                                   .SelectMany(pl => pl.Execucaos)
                                   .Where(e => e.Status != Core.Enum.Execucao.Status.SUCESSO.ToString())
                                   .GroupBy(e => e.DataConfirmacao)
                                        .Select(grupoDoDia => new PacienteDto.ExecucaoDto
                                        {
                                            IdPlanejamento = src.Id,
                                            Data = grupoDoDia.Key.ToString("yyyy-MM-dd"),
                                            Quantidade = grupoDoDia.Count(),
                                            Status = grupoDoDia.Any(e => e.Status == "FALHA") ? "FALHA" : "ATRASO"
                                        })
                            ));

            CreateMap<Paciente, PacienteDetailsDto>()
                            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo.ToString()))
                            .ForMember(dest => dest.Escolaridade, opt => opt.MapFrom(src => src.Escolaridade.ToString()))
                            .ForMember(dest => dest.Alergias, opt => opt.MapFrom(src => src.Alergia))
                            .ForMember(dest => dest.Deficiencias, opt => opt.MapFrom(src => src.Deficiencia));

            CreateMap<Deficiencium, PacienteDeficienciaDto>();

            CreateMap<Alergium, PacienteAlergiaDto>()
                .ForMember(dest => dest.MedicamentoNome,
                           opt => opt.MapFrom(src => src.IdMedicamentoNavigation != null ? src.IdMedicamentoNavigation.Nome : null));

            CreateMap<Vinculo, VinculoDto>()
                            .ForMember(dest => dest.Parentesco, opt => opt.MapFrom(src => src.Parentesco.ToString()));
        }
    }
}
