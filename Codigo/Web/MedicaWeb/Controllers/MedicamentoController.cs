using AutoMapper;
using Core;
using Core.Dto;
using Core.Dto.PacienteDto;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace MedicaWeb.Controllers
{
    public class MedicamentoController : Controller
    {
        private readonly IMedicamentoService medicamentoService;
        private readonly IPacienteService pacienteService;
        private readonly IMapper mapper;

        public MedicamentoController(IMedicamentoService medicamentoService, IPacienteService pacienteService, IMapper mapper)
        {
            this.medicamentoService = medicamentoService;
            this.pacienteService = pacienteService;
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
            var medicamento = medicamentoService.Get((uint)id);
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
        public ActionResult Create(MedicamentoViewModel medicamentoModel)
        {
            medicamentoModel.IdCuidador = 1;
            var fotoMedicamento = Request.Form.Files["fotoMedicamento"];
            if (fotoMedicamento != null && fotoMedicamento.Length > 0)
            {
                using var ms = new MemoryStream();
                fotoMedicamento.CopyTo(ms);
                medicamentoModel.Foto = ms.ToArray();
            }
            if (ModelState.IsValid)
            {
                var medicamento = mapper.Map<Medicamento>(medicamentoModel);
                medicamentoService.Create(medicamento);

            }
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Edit/5
        public ActionResult Edit(int id)
        {
            var medicamento = medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MedicamentoViewModel medicamentoModel)
        {
            medicamentoModel.IdCuidador = 1;
            var fotoMedicamento = Request.Form.Files["fotoMedicamento"];
            if (fotoMedicamento != null && fotoMedicamento.Length > 0)
            {
                using var ms = new MemoryStream();
                fotoMedicamento.CopyTo(ms);
                medicamentoModel.Foto = ms.ToArray();
            }
            else
            {
                var medicamentoAtual = medicamentoService.Get((uint)id);
                medicamentoModel.Foto = medicamentoAtual!.Foto;
            }
            if (ModelState.IsValid)
            {
                var medicamento = mapper.Map<Medicamento>(medicamentoModel);
                medicamentoService.Edit(medicamento);
            }
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
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
            medicamentoService.Delete((uint)id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Index));
        }
    }
}
