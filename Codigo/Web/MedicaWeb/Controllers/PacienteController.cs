using AutoMapper;
using Core;
using Core.Dto.Paciente;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;


namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador, Cuidador")]
    public class PacienteController : Controller
    {
        private readonly IPacienteService pacienteService;
        private readonly IVinculoService vinculoService;
        private readonly IMedicamentoService medicamentoService;
        private readonly IMapper mapper;

        public PacienteController(
            IPacienteService pacienteService,
            IVinculoService vinculoService,
            IMedicamentoService medicamentoService,
            IMapper mapper
        )
        {
            this.pacienteService = pacienteService;
            this.vinculoService = vinculoService;
            this.medicamentoService = medicamentoService;
            this.mapper = mapper;
        }

        // GET: PacienteController
        public async Task<IActionResult> Index()
        {
            var pacientes = await pacienteService.GetAll(User.GetId());
            var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return View(pacienteDtos);
        }

        // GET: PacienteController/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            var paciente = await pacienteService.Get(id);
            var pacienteDetailsDto = mapper.Map<PacienteDetailsDto>(paciente);
            return View(pacienteDetailsDto);
        }

        // GET: PacienteController/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var medicamentos = await medicamentoService.GetAll(User.GetId());
            ViewBag.Medicamentos = new SelectList(medicamentos, "Id", "Nome");
            return View(new PacienteDetailsDto());
        }

        // POST: PacienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteDetailsDto pacienteDetailsDto)
        {
            if (ModelState.IsValid)
            {
                var pacienteModel = mapper.Map<Paciente>(pacienteDetailsDto);
                var vinculo = new Vinculo
                {
                    IdCuidador = User.GetId(),
                    Parentesco = pacienteDetailsDto.Vinculo.Parentesco.ToString()!
                };
                await pacienteService.Create(pacienteModel, vinculo);
                NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
                return RedirectToAction(nameof(Index));
            }
            var medicamentos = await medicamentoService.GetAll(User.GetId());
            ViewBag.Medicamentos = new SelectList(medicamentos, "Id", "Nome");
            return View(pacienteDetailsDto);
        }

        // GET: PacienteController/Edit/5
        public async Task<IActionResult> Edit(uint id)
        {
            var paciente = await pacienteService.Get(id);
            var pacienteDetailsDto = mapper.Map<PacienteDetailsDto>(paciente);
            return View(pacienteDetailsDto);
        }

        // POST: PacienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, PacienteDetailsDto pacienteDetailsDto)
        {
            pacienteDetailsDto.Id = id;
            var pacienteModel = mapper.Map<Paciente>(pacienteDetailsDto);
            await pacienteService.Edit(pacienteModel);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // POST: PacienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            await vinculoService.DeleteByPaciente(id);
            await pacienteService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}