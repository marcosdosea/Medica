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
                                   .SelectMany(pl => pl.Execucaos, (pl, e) => new { Planejamento = pl, Execucao = e })
                                   .Where(e => e.Execucao.Status != Core.Enum.Execucao.Status.SUCESSO.ToString())
                                   .Select(e => new PacienteDto.ExecucaoDto
                                   {
                                       Data = e.Execucao.DataConfirmacao.ToString("yyyy-MM-dd"),
                                       Status = e.Execucao.Status,
                                       NomeMedicamentoHora = $"{e.Planejamento.Hora:hh\\:mm} - " +
                                                             $"{e.Planejamento.IdMedicamentoNavigation.Nome}"
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
