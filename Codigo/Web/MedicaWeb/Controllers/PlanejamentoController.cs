using AutoMapper;
using Core;
using Core.Dto.Planejamento;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

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

        // GET: PlanejamentoController
        public async Task<IActionResult> Index()
        {
            var planejamentos = await planejamentoService.GetAll();
            var planejamentoDtos = mapper.Map<IEnumerable<PlanejamentoDto>>(planejamentos);
            return View(planejamentoDtos);
        }

        // GET: PlanejamentoController/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            var planejamentoViewModel = mapper.Map<PlanejamentoViewModel>(planejamento);
            return View(planejamentoViewModel);
        }

        // GET: PlanejamentoController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlanejamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanejamentoViewModel planejamentoViewModel)
        {
            var planejamentoModel = mapper.Map<Planejamento>(planejamentoViewModel);
            await planejamentoService.Create(planejamentoModel);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
        }

        // GET: PlanejamentoController/Edit/5
        public async Task<IActionResult> Edit(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            var planejamentoViewModel = mapper.Map<PlanejamentoViewModel>(planejamento);
            return View(planejamentoViewModel);
        }

        // POST: PlanejamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, PlanejamentoViewModel planejamentoViewModel)
        {
            planejamentoViewModel.Id = id;
            var planejamentoModel = mapper.Map<Planejamento>(planejamentoViewModel);
            await planejamentoService.Edit(planejamentoModel);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // POST: PlanejamentoController/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            await planejamentoService.Delete(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // POST: PlanejamentoController/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(uint id)
        {
            await planejamentoService.Activate(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.AtivacaoSucesso);
            return RedirectToAction(nameof(Index));
        }
    }
}