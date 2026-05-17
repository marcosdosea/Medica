using AutoMapper;
using Core;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

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
        public ActionResult Index()
        {
            var listaPacientes = pacienteService.GetAll();
            var listaPacientesModel = mapper.Map<List<PacienteViewModel>>(listaPacientes);

            return View(listaPacientesModel);
        }

        // GET: PacienteController/Details/5
        public ActionResult Details(int id)
        {
            var paciente = pacienteService.Get((uint)id);
            if (paciente == null)
            {
                return NotFound();
            }

            var pacienteModel = mapper.Map<PacienteViewModel>(paciente);
            return View(pacienteModel);
        }

        // GET: PacienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PacienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PacienteViewModel pacienteModel)
        {
            if (!ModelState.IsValid)
            {
                return View(pacienteModel);
            }

            var paciente = mapper.Map<Paciente>(pacienteModel);
            pacienteService.Create(paciente);

            return RedirectToAction(nameof(Index));
        }

        // GET: PacienteController/Edit/5
        public ActionResult Edit(int id)
        {
            var paciente = pacienteService.Get((uint)id);
            if (paciente == null)
            {
                return NotFound();
            }

            var pacienteModel = mapper.Map<PacienteViewModel>(paciente);
            return View(pacienteModel);
        }

        // POST: PacienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PacienteViewModel pacienteModel)
        {
            if (id != pacienteModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(pacienteModel);
            }

            var paciente = mapper.Map<Paciente>(pacienteModel);
            pacienteService.Edit(paciente);

            return RedirectToAction(nameof(Index));
        }

        // GET: PacienteController/Delete/5
        public ActionResult Delete(int id)
        {
            var paciente = pacienteService.Get((uint)id);
            if (paciente == null)
            {
                return NotFound();
            }

            var pacienteModel = mapper.Map<PacienteViewModel>(paciente);
            return View(pacienteModel);
        }

        // POST: PacienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PacienteViewModel pacienteModel)
        {
            pacienteService.Delete((uint)id);
            return RedirectToAction(nameof(Index));
        }
    }
}