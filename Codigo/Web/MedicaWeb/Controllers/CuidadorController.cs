using AutoMapper;
using Core.Dto.Usuario;
using Core.Dto.Planejamento;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using MedicaWeb.Areas.Identity.Data;

namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CuidadorController : Controller
    {
        private ICuidadorService cuidadorService;
        private readonly UserManager<Usuario> userManager;
        private readonly IMapper mapper;

        public CuidadorController(ICuidadorService cuidadorService, IMapper mapper, UserManager<Usuario> userManager)
        {
            this.cuidadorService = cuidadorService;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        // GET: CuidadorController
        public async Task<IActionResult> Index()
        {
            var cuidadores = await cuidadorService.GetAll();
            var admins = await userManager.GetUsersInRoleAsync("Administrador");
            var cpfsAdmins = admins.Select(a => a.UserName).ToHashSet();
            var cuidadoresDto = mapper.Map<IEnumerable<UsuarioDto>>(cuidadores, opt =>
            {
                opt.Items["CpfsAdmins"] = cpfsAdmins;
            });
            return View(cuidadoresDto);
        }
    }
}
