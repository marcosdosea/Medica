using AutoMapper;
using Core.Dto.Paciente;
using Core.Service;
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

            if (paciente == null || paciente.Ativo == "N")
            {
                return NotFound(new
                {
                    sucesso = false,
                    mensagem = "Paciente não encontrado ou inativo.",
                    timestamp = DateTime.UtcNow
                });
            }

            // O AutoMapper converte a entidade Paciente para o PacienteMobileDto
            var pacienteDto = _mapper.Map<PacienteMobileDto>(paciente);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Paciente encontrado com sucesso.",
                timestamp = DateTime.UtcNow,
                data = pacienteDto
            });
        }
    }
}