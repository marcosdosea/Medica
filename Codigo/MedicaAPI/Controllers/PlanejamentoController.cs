using AutoMapper;
using Core.Dto.Planejamento;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanejamentoController : ControllerBase
    {
        private readonly IPlanejamentoService _planejamentoService;
        private readonly IMapper _mapper;

        public PlanejamentoController(IPlanejamentoService planejamentoService, IMapper mapper)
        {
            _planejamentoService = planejamentoService;
            _mapper = mapper;
        }

        [HttpGet("mobile/{id}")]
        public async Task<IActionResult> GetPlanejamentoAlarme(uint id)
        {
            var planejamento = await _planejamentoService.Get(id);

            if (planejamento == null || planejamento.Ativo == "N")
            {
                return NotFound(new
                {
                    sucesso = false,
                    mensagem = "Planejamento não encontrado.",
                    timestamp = DateTime.UtcNow
                });
            }

            // Mapeia para o DTO focado no mobile (com ícones, quantidade, etc.)
            var planejamentoDto = _mapper.Map<PlanejamentoMobileDto>(planejamento);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Planejamento encontrado com sucesso.",
                timestamp = DateTime.UtcNow,
                data = planejamentoDto
            });
        }
    }
}