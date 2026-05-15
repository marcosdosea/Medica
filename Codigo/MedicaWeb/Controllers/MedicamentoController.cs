using AutoMapper;
using Core;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaWeb.Controllers
{
    public class MedicamentoController : Controller
    {
        private readonly IMedicamentoService medicamentoService;
        private readonly IMapper mapper;
        public MedicamentoController(IMedicamentoService medicamentoService, IMapper mapper)
        {
            this.medicamentoService = medicamentoService;
            this.mapper = mapper;
        }

        // GET: MedicamentoController
        public ActionResult Index()
        {
            var listaMedicamentos = medicamentoService.GetAll();
            var listaMedicamentosModel = mapper.Map<List<MedicamentoViewModel>>(listaMedicamentos);
            return View(listaMedicamentosModel);
        }

        // GET: MedicamentoController/Details/5
        public ActionResult Details(int id)
        {
            var medicamento = medicamentoService.Get((uint) id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View();
        }

        // GET: MedicamentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedicamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicamentoViewModel medicamentoModel)
        {
            if (ModelState.IsValid)
            {
                var medicamento = mapper.Map<Medicamento>(medicamentoModel);
                medicamentoService.Create(medicamento);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Edit/5
        public ActionResult Edit(int id)
        {
            var medicamento = medicamentoService.Get((uint) id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MedicamentoViewModel medicamentoModel)
        {
            if (ModelState.IsValid)
            {
                var medicamento = mapper.Map<Medicamento>(medicamentoModel);
                medicamentoService.Edit(medicamento);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Delete/5
        public ActionResult Delete(int id)
        {
            var medicamento = medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, MedicamentoViewModel _)
        {
            medicamentoService.Delete((uint) id);
            return RedirectToAction(nameof(Index));
        }
    }
}
