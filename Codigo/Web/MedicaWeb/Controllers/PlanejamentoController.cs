using AutoMapper;
using Core;
using Core.Dto.Planejamento;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicaWeb.Controllers
{
    public class PlanejamentoController : Controller
    {
        private readonly IPlanejamentoService planejamentoService;
        private readonly IMapper mapper;

        public PlanejamentoController(IPlanejamentoService planejamentoService, IMapper mapper)
        {
            this.planejamentoService = planejamentoService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var planejamentos = await planejamentoService.GetAll();
            var planejamentoDtos = mapper.Map<IEnumerable<PlanejamentoDto>>(planejamentos);
            return View(planejamentoDtos);
        }

        public async Task<IActionResult> Details(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            var planejamentoViewModel = mapper.Map<PlanejamentoViewModel>(planejamento);
            return View(planejamentoViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanejamentoViewModel planejamentoViewModel)
        {
            var planejamentoModel = mapper.Map<Planejamento>(planejamentoViewModel);
            await planejamentoService.Create(planejamentoModel);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            var planejamentoViewModel = mapper.Map<PlanejamentoViewModel>(planejamento);
            return View(planejamentoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, PlanejamentoViewModel planejamentoViewModel)
        {
            planejamentoViewModel.Id = id;
            var planejamentoModel = mapper.Map<Planejamento>(planejamentoViewModel);
            await planejamentoService.Edit(planejamentoModel);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            await planejamentoService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}