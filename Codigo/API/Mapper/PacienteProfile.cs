using AutoMapper;
using Core;
using Core.Dto.Paciente;

namespace MedicaAPI.Mapper
{
    public class PacienteProfile : Profile
    {
        public PacienteProfile()
        {
            CreateMap<Paciente, PacienteMobileDetailsDto>();
        }
    }
}
