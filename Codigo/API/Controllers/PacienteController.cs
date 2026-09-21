using AutoMapper;
using Core.Dto.Paciente;
using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;
        private readonly IMapper _mapper;

        public PacienteController(IPacienteService pacienteService, IMapper mapper)
        {
            _pacienteService = pacienteService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPacienteMobile(uint id)
        {
            var paciente = await _pacienteService.Get(id);
            var pacienteDto = _mapper.Map<PacienteMobileDetailsDto>(paciente);
            return Ok(DefaultGenericResponse.Success(pacienteDto,
                "Paciente encontrado com sucesso.")
            );
        }
    }
}