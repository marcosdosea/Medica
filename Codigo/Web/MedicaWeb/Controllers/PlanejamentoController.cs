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
    [Authorize(Roles = "Cuidador")]
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

        // GET: PlanejamentoController
        public async Task<IActionResult> Index()
        {
            var idCuidador = User.GetId();
            var planejamentos = await planejamentoService.GetAll(idCuidador);
            var planejamentoDtos = mapper.Map<IEnumerable<PlanejamentoDto>>(planejamentos);
            return View(planejamentoDtos);
        }

        // GET: PlanejamentoController/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            var planejamentoDetailsDto = mapper.Map<PlanejamentoDetailsDto>(planejamento);
            return View(planejamentoDetailsDto);
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
            return RedirectToAction(nameof(Index));
        }

        // GET: PlanejamentoController/Edit/5
        public async Task<IActionResult> Edit(uint id)
        {
            var planejamento = await planejamentoService.Get(id);
            uint idCuidador = User.GetId();
            var medicamentos = await medicamentoService.GetAll(idCuidador);
            ViewBag.Medicamentos = new SelectList(medicamentos, "Id", "Nome", planejamento!.IdMedicamento);
            var paciente = await pacienteService.Get(planejamento.IdPaciente);
            ViewBag.NomePaciente = paciente?.Nome ?? "Paciente";
            ViewBag.FotoPaciente = paciente?.Foto;
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
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
        }
    }
}