using AutoMapper;
using Core;
using Core.Dto.PacienteDto;
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

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var pacientes = await pacienteService.GetAsync(searchTerm);
            var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return View(pacienteDtos);
        }

        public async Task<IActionResult> Details(uint id)
        {
            var paciente = await pacienteService.GetAsync(id);
            if (paciente == null) return NotFound();

            var pacienteDetailsDto = mapper.Map<PacienteDetailsDto>(paciente);
            return View(pacienteDetailsDto);
        }

        public IActionResult Create()
        {
            return View(new PacienteDetailsDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteDetailsDto pacienteDetailsDto)
        {
            var pacienteModel = mapper.Map<Paciente>(pacienteDetailsDto);

            await pacienteService.CreateAsync(pacienteModel);

            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(uint id)
        {
            var paciente = await pacienteService.GetAsync(id);
            if (paciente == null) return NotFound();

            var dto = mapper.Map<PacienteDetailsDto>(paciente);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, PacienteDetailsDto dto)
        {
            dto.Id = id;

            var pacienteModel = mapper.Map<Paciente>(dto);

            await pacienteService.EditAsync(pacienteModel);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            await pacienteService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}