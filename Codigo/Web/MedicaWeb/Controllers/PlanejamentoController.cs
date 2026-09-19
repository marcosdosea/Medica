using AutoMapper;
using Core;
using Core.Dto.Paciente;
using Core.Dto.Planejamento;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;

namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador, Cuidador")]
    public class PlanejamentoController : Controller
    {
        private readonly IPlanejamentoService planejamentoService;
        private readonly IPacienteService pacienteService;
        private readonly IMedicamentoService medicamentoService;
        private readonly IMapper mapper;

        public PlanejamentoController(
            IPlanejamentoService planejamentoService, IPacienteService pacienteService, IMedicamentoService medicamentoService, IMapper mapper)
        {
            this.planejamentoService = planejamentoService;
            this.pacienteService = pacienteService;
            this.medicamentoService = medicamentoService;
            this.mapper = mapper;
        }

        // GET: PlanejamentoController/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            uint idCuidador = User.GetId();
            var pacientesEntidades = await pacienteService.GetAll(idCuidador);
            ViewBag.Pacientes = mapper.Map<IEnumerable<PacienteDto>>(pacientesEntidades);
            var medicamentos = await medicamentoService.GetAll(idCuidador);
            ViewBag.Medicamentos = new SelectList(medicamentos, "Id", "Nome");
            var planejamentos = await planejamentoService.GetAll(idCuidador);
            var planejamentosAtivos = planejamentos.Where(p => p.Ativo == "S");
            ViewBag.PlanejamentosExistentes = mapper.Map<IEnumerable<PlanejamentoItemDto>>(planejamentosAtivos);
            return View(new PlanejamentoViewModel());
        }

        // POST: PlanejamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanejamentoViewModel planejamentoViewModel)
        {
            var planejamentos = mapper.Map<IEnumerable<Planejamento>>(planejamentoViewModel);
            await planejamentoService.Create(planejamentos);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Create));
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
            return RedirectToAction(nameof(Create));
        }

        // POST: PlanejamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            await planejamentoService.Delete(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Create));
        }
    }
}