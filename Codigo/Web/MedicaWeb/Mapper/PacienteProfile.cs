using AutoMapper;
using Core;
using Core.Dto.Paciente;
using Core.Enum.Paciente;
using static Core.Dto.Paciente.PacienteDetailsDto;
using Util;

namespace MedicaWeb.Mapper
{
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            CreateMap<PacienteDetailsDto, Paciente>()
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => "S"))
                .ForMember(dest => dest.NomeResponsavel, opt => opt.MapFrom(src => src.NomeTelefoneResponsavel))
                .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo.ToString()))
                .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => FotoHelper.ConverterFoto(src.Foto)))
                .ForMember(dest => dest.Alergia, opt => opt.MapFrom(src =>
                    src.Alergias != null
                        ? src.Alergias.Select(a => new Alergium
                        {
                            Tipo = a.Tipo,
                            IdMedicamento = a.IdMedicamento,
                            Descricao = a.Descricao
                        }).ToList()
                        : new List<Alergium>()));

            CreateMap<Paciente, PacienteDto>()
                .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo ?? "M"));

            CreateMap<Paciente, PacienteDetailsDto>()
                .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src =>
                    src.Sexo == "F" ? Sexo.F : Sexo.M))
                .ForMember(dest => dest.Escolaridade, opt => opt.MapFrom(src =>
                    src.Escolaridade != null
                        ? Enum.Parse<Escolaridade>(src.Escolaridade.ToString())
                        : (Escolaridade?)null))
                .ForMember(dest => dest.Foto, opt => opt.Ignore())
                .ForMember(dest => dest.FotoBytes, opt => opt.MapFrom(src => src.Foto))
                .ForMember(dest => dest.Alergias, opt => opt.MapFrom(src => src.Alergia))
                .ForMember(dest => dest.Vinculo, opt => opt.MapFrom(src => src.Vinculos.FirstOrDefault()));

            CreateMap<Alergium, PacienteAlergiaDto>()
                .ForMember(dest => dest.MedicamentoNome, opt => opt.MapFrom(src =>
                    src.IdMedicamentoNavigation != null ? src.IdMedicamentoNavigation.Nome : null));

            CreateMap<Vinculo, VinculoDto>()
                .ForMember(dest => dest.IdCuidador, opt => opt.MapFrom(src => (int)src.IdCuidador))
                .ForMember(dest => dest.Parentesco, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.Parentesco)
                        ? Enum.Parse<Parentesco>(src.Parentesco, true)
                        : (Parentesco?)null));
        }
    }
}