using AutoMapper;
using Core;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Util;

namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador, Cuidador")]
    public class MedicamentoController : Controller
    {
        private readonly IMedicamentoService medicamentoService;
        private readonly IMapper mapper;

        public MedicamentoController(
            IMedicamentoService medicamentoService,
            IMapper mapper)
        {
            this.medicamentoService = medicamentoService;
            this.mapper = mapper;
        }

        // GET: MedicamentoController
        public async Task<ActionResult> Index()
        {
            var medicamentos = await medicamentoService.GetAll(User.GetId());
            var medicamentoViewModels = mapper.Map<List<MedicamentoViewModel>>(medicamentos);
            return View(medicamentoViewModels);
        }

        // GET: MedicamentoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var medicamento = await medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // GET: MedicamentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedicamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicamentoViewModel medicamentoModel)
        {
            if (ModelState.IsValid)
            {
                medicamentoModel.IdCuidador = User.GetId();
                var medicamento = mapper.Map<Medicamento>(medicamentoModel);
                await medicamentoService.Create(medicamento);
                NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
                return RedirectToAction(nameof(Index));
            }
            return View(medicamentoModel);
        }

        // GET: MedicamentoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var medicamento = await medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicamentoViewModel medicamentoModel)
        {
            if (!ModelState.IsValid)
            {
                return View(medicamentoModel);
            }
            medicamentoModel.IdCuidador = User.GetId();
            var medicamento = await medicamentoService.Get((uint)id);
            mapper.Map(medicamentoModel, medicamento);
            await medicamentoService.Edit(medicamento!);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var medicamento = await medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, MedicamentoViewModel _)
        {
            await medicamentoService.Delete((uint)id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Index));
        }
    }
}

