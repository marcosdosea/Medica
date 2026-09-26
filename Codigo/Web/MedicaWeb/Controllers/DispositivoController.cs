using AutoMapper;
using Core.Dto.Dispositivo;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Util;

namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador, Cuidador")]
    public class DispositivoController : Controller
    {
        private readonly IDispositivoService dispositivoService;
        private readonly IAuthService authService;
        private readonly IMapper mapper;

        public DispositivoController(
            IDispositivoService dispositivoService,
            IAuthService authService,
            IMapper mapper)
        {
            this.dispositivoService = dispositivoService;
            this.authService = authService;
            this.mapper = mapper;
        }

        // GET: DispositivoController
        public async Task<IActionResult> Index()
        {
            var dispositivos = await dispositivoService.GetAll(User.GetId());
            var dispositivosDto = mapper.Map<IEnumerable<DispositivoDto>>(dispositivos);
            return View(dispositivosDto);
        }

        // GET: DispositivoController/ObterToken/5
        [HttpGet]
        public async Task<IActionResult> ObterToken(uint id)
        {
            var token = await authService.GerarTokenPareamento(id);
            return Json(new { token });
        }
    }
}
