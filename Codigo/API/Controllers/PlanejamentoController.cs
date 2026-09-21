using AutoMapper;
using Core.Dto.Planejamento;
using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PlanejamentoController : ControllerBase
    {
        private readonly IPlanejamentoService planejamentoService;
        private readonly IMapper mapper;

        public PlanejamentoController(IPlanejamentoService planejamentoService, IMapper mapper)
        {
            this.planejamentoService = planejamentoService;
            this.mapper = mapper;
        }

        [HttpGet("paciente/{idPaciente}")]
        public async Task<IActionResult> SincronizarPlanejamentos(uint idPaciente, [FromQuery] DateTime? ultimaSincronizacao)
        {
            var planejamentos = await planejamentoService.GetAllByPaciente(idPaciente, ultimaSincronizacao);
            var idsParaExcluir = await planejamentoService.GetIdsExcluidosByPaciente(idPaciente, ultimaSincronizacao);
            var responseDto = new PlanejamentoMobileDto
            {
                Planejamentos = mapper.Map<List<PlanejamentoMobileResponseDto>>(planejamentos),
                Excluir = [.. idsParaExcluir],
                Sincronizacao = DateTime.UtcNow
            };
            return Ok(DefaultGenericResponse<PlanejamentoMobileDto>.Success(
                responseDto,
                "Planejamentos encontrados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            var planejamentoDto = mapper.Map<PlanejamentoMobileDetailsDto>(planejamento);
            return Ok(DefaultGenericResponse<PlanejamentoMobileDetailsDto>.Success(
                planejamentoDto,
                "Planejamento encontrado com sucesso."));
        }
    }
}