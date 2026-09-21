using AutoMapper;
using Core;
using Core.Dto.Execucao;
using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExecucaoController : ControllerBase
    {
        private readonly IExecucaoService _execucaoService;
        private readonly IMapper _mapper;

        public ExecucaoController(IExecucaoService execucaoService, IMapper mapper)
        {
            _execucaoService = execucaoService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarExecucao([FromBody] ExecucaoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(DefaultGenericResponse.Error(null, "Dados inválidos fornecidos pelo aplicativo."));
            }
            var execucao = _mapper.Map<Execucao>(request);
            var execucaoId = await _execucaoService.Create(execucao);
            return StatusCode(StatusCodes.Status201Created, DefaultGenericResponse<uint>.Success(
                execucaoId,
                "Medicamento tomado com sucesso e estoque atualizado."));
        }
    }
}