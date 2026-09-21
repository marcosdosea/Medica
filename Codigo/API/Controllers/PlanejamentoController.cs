using AutoMapper;
using Core.Dto.Planejamento;
using Core.Service;
using MedicaAPI.Models;
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

        [HttpGet("paciente/{idPaciente}")]
        public async Task<IActionResult> SincronizarPlanejamentos(uint idPaciente, [FromQuery] DateTime? ultimaSincronizacao)
        {
            var planejamentos = await _planejamentoService.GetAllByPaciente(idPaciente, ultimaSincronizacao);
            var idsParaExcluir = await _planejamentoService.GetIdsExcluidosByPaciente(idPaciente, ultimaSincronizacao);
            var responseDto = new PlanejamentoMobileDto
            {
                Planejamentos = _mapper.Map<List<PlanejamentoMobileResponseDto>>(planejamentos),
                Excluir = [.. idsParaExcluir],
                Sincronizacao = DateTime.UtcNow
            };
            return Ok(DefaultGenericResponse<PlanejamentoMobileDto>.Success(
                responseDto,
                "Planejamentos recuperados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(uint id)
        {
            var planejamento = await _planejamentoService.Get(id);
            var planejamentoDto = _mapper.Map<PlanejamentoMobileDetailsDto>(planejamento);
            return Ok(DefaultGenericResponse<PlanejamentoMobileDetailsDto>.Success(
                planejamentoDto,
                "Planejamento encontrado com sucesso."));
        }
    }
}