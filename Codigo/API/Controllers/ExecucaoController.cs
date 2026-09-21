using AutoMapper;
using Core;
using Core.Dto.Execucao;
using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExecucaoController : ControllerBase
    {
        private readonly IExecucaoService execucaoService;
        private readonly IMapper mapper;

        public ExecucaoController(IExecucaoService execucaoService, IMapper mapper)
        {
            this.execucaoService = execucaoService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarExecucao([FromBody] ExecucaoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(DefaultGenericResponse.Error(null, "Dados inválidos."));
            }
            var execucao = mapper.Map<Execucao>(request);
            var execucaoId = await execucaoService.Create(execucao);
            return StatusCode(StatusCodes.Status201Created, DefaultGenericResponse<uint>.Success(
                execucaoId,
                "Medicamento tomado com sucesso e estoque atualizado."));
        }
    }
}