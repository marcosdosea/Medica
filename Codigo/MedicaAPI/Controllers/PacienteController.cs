using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Mvc;


namespace MedicaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService pacienteService;   
        private readonly IMapper mapper;
        public PacienteController(IPacienteService pacienteService, IMapper mapper)
        {
            this.mapper = mapper;
            this.pacienteService = pacienteService;
        }

        [HttpGet(Name = "Paciente")]
        public async Task<ActionResult> GetAsync()
        {
            var paciente = await pacienteService.GetMobileAsync();
            return Ok(paciente);
        }
    }
}
