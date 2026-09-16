using AutoMapper;
using Core;
using Core.Dto.Usuario;
using System.Collections.Generic;

namespace MedicaWeb.Mapper
{
    public class CuidadorProfile : Profile
    {
        public CuidadorProfile()
        {
            CreateMap<Cuidador, UsuarioDto>()
                .ForMember(dest => dest.QuantidadePacientes,
                           opt => opt.MapFrom(src => src.Vinculos != null ? src.Vinculos.Count : 0))
                .ForMember(dest => dest.Perfil, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    if (context.Items.TryGetValue("CpfsAdmins", out var item) &&
                        item is HashSet<string> cpfsAdmins)
                    {
                        return cpfsAdmins.Contains(src.Cpf) ? "Administrador" : "Cuidador";
                    }

                    return "Cuidador";
                }));
        }
    }
}