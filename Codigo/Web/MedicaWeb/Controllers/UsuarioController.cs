using AutoMapper;
using Core.Dto.Cuidador;
using Core.Dto.Planejamento;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private ICuidadorService cuidadorService;
        private readonly IMapper mapper;

        public UsuarioController(ICuidadorService cuidadorService, IMapper mapper)
        {
            this.cuidadorService = cuidadorService;
            this.mapper = mapper;
        }

        // GET: UsuarioController
        public async Task<IActionResult> Index()
        {
            var cuidadores = await cuidadorService.GetAll();
            var cuidadoresDto = mapper.Map<IEnumerable<CuidadorDto>>(cuidadores);
            return View(cuidadoresDto);

        }
    }
}
