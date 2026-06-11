using AutoMapper;
using Core;
using Core.Dto.Paciente;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using Microsoft.AspNetCore.Mvc;


namespace MedicaWeb.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteService pacienteService;
        private readonly IMapper mapper;

        public PacienteController(IPacienteService pacienteService, IMapper mapper)
        {
            this.pacienteService = pacienteService;
            this.mapper = mapper;
        }

        // GET: PacienteController
        public async Task<IActionResult> Index()
        {
            var pacientes = await pacienteService.GetAll();
            var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return View(pacienteDtos);
        }

        // GET: PacienteController/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            uint idCuidador = 1;
            var paciente = await pacienteService.Get(id);
            var vinculos = paciente!.Vinculos
                                    .FirstOrDefault(v => v.IdCuidador == idCuidador);
            var pacienteDetailsDto = mapper.Map<PacienteDetailsDto>(paciente);
            pacienteDetailsDto.Vinculo = mapper.Map<PacienteDetailsDto.VinculoDto>(vinculos);
            return View(pacienteDetailsDto);
        }

        // GET: PacienteController/Create
        public IActionResult Create()
        {
            return View(new PacienteDetailsDto());
        }

        // POST: PacienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteDetailsDto pacienteDetailsDto)
        {
            var pacienteModel = mapper.Map<Paciente>(pacienteDetailsDto);
            await pacienteService.Create(pacienteModel);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(uint id, PacienteDetailsDto dto)
        {
            dto.Id = id;
            var pacienteModel = mapper.Map<Paciente>(dto);
            await pacienteService.Edit(pacienteModel);
            return RedirectToAction(nameof(Index));
        }

        // GET: PacienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            await pacienteService.Delete(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // POST: PacienteController/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(uint id)
        {
            await pacienteService.Activate(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.AtivacaoSucesso);
            return RedirectToAction(nameof(Index));
        }
    }
}