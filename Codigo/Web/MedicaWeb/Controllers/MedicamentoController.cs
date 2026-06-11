using AutoMapper;
using Core;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> Index()
        {
            var listaMedicamentos = await medicamentoService.GetAll();
            var listaMedicamentosModel = mapper.Map<List<MedicamentoViewModel>>(listaMedicamentos);
            return View(listaMedicamentosModel);
        }

        // GET: MedicamentoController/Details/5
        public async Task<ActionResult> Details(int id)
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
        public async Task<ActionResult> Create(MedicamentoViewModel medicamentoModel)
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
                await medicamentoService.Create(medicamento);

            }
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var medicamento = await medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, MedicamentoViewModel medicamentoModel)
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
                var medicamentoAtual = await medicamentoService.Get((uint)id);
                medicamentoModel.Foto = medicamentoAtual!.Foto;
            }
            if (ModelState.IsValid)
            {
                var medicamento = mapper.Map<Medicamento>(medicamentoModel);
                await medicamentoService.Edit(medicamento);
            }
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // POST: MedicamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, MedicamentoViewModel _)
        {
            await medicamentoService.Delete((uint)id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Index));
        }

        // POST: MedicamentoController/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(uint id)
        {
            await medicamentoService.Activate(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.AtivacaoSucesso);
            return RedirectToAction(nameof(Index));
        }
    }
}
