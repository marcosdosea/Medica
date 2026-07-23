using AutoMapper;
using Core;
using Core.Dto.Paciente;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace MedicaWeb.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IPacienteService pacienteService;
        private readonly ICuidadorService cuidadorService;
        private readonly IMapper mapper;
        private readonly ILogger<HomeController> logger;

        public HomeController(
            IPacienteService pacienteService,
            ICuidadorService cuidadorService,
            IMapper mapper,
            ILogger<HomeController> logger
        )
        {
            this.pacienteService = pacienteService;
            this.cuidadorService = cuidadorService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Cuidador"))
            {
                var idCuidador = GetIdUserLogado();
                var pacientes = await pacienteService.GetAll(idCuidador);
                var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
                return View(pacienteDtos);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetailsExecucoes(int mes, int ano)
        {
            var idCuidador = GetIdUserLogado();
            var pacientes = await pacienteService.GetAll(idCuidador, ano, mes);
            var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return Json(pacienteDtos);
        }
    }
}
