using AutoMapper;
using Core;
using Core.Dto.Execucao;
using Core.Service;
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
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "Dados inválidos enviados pelo aplicativo.",
                    timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var execucao = _mapper.Map<Execucao>(request);

                await _execucaoService.Create(execucao);

                return StatusCode(201, new
                {
                    sucesso = true,
                    mensagem = "Execução inserida com sucesso.",
                    timestamp = DateTime.UtcNow,
                    data = (object)null!
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = $"Erro ao registrar a execução: {ex.Message}",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}