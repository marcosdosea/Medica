using AutoMapper;
using Core.Dto.Paciente;
using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService pacienteService;
        private readonly IMapper mapper;

        public PacienteController(IPacienteService pacienteService, IMapper mapper)
        {
            this.pacienteService = pacienteService;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPacienteMobile(uint id)
        {
            var paciente = await pacienteService.Get(id);
            var pacienteDto = mapper.Map<PacienteMobileDetailsDto>(paciente);
            return Ok(DefaultGenericResponse.Success(pacienteDto,
                "Paciente encontrado com sucesso.")
            );
        }
    }
}