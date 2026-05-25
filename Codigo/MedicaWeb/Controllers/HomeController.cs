using AutoMapper;
using Core.Dto;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MedicaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPacienteService pacienteService;
        private readonly IMapper mapper;
        private readonly ILogger<HomeController> logger;

        public HomeController(IPacienteService pacienteService, IMapper mapper, ILogger<HomeController> logger)
        {
            this.pacienteService = pacienteService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var pacientes = await pacienteService.GetAsync(searchTerm);
            var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes); 
            return View(pacienteDtos);
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
    }
}
