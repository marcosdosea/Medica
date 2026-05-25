using AutoMapper;
using Core;
using Core.Dto;
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
        private readonly IPrescricaoService prescricaoService;
        private readonly IPacienteService pacienteService;
        private readonly IMapper mapper;

        public MedicamentoController(IMedicamentoService medicamentoService, IPrescricaoService prescricaoService, IPacienteService pacienteService, IMapper mapper)
        {
            this.medicamentoService = medicamentoService;
            this.prescricaoService = prescricaoService;
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
            var prescricoes = prescricaoService.GetAll((uint)id);
            medicamentoModel.Pacientes = prescricoes
                                            .Select(p => mapper.Map<PacienteDto>(p.IdPacienteNavigation))
                                            .ToList();
            return View(medicamentoModel);
        }

        // GET: MedicamentoController/Create
        public ActionResult Create()
        {
            var pacientes = pacienteService.GetAsync();
            var pacientesDto = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            var medicamentoModel = new MedicamentoViewModel
            {
                Pacientes = pacientesDto
            };
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicamentoViewModel medicamentoModel)
        {
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
                foreach (var idPaciente in medicamentoModel.PacientesSelecionados!)
                {
                    var novaPrescricao = new Prescricao
                    {
                        IdMedicamento = medicamento.Id,
                        IdPaciente = idPaciente
                    };
                    prescricaoService.Create(novaPrescricao);
                }

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Edit/5
        public ActionResult Edit(int id)
        {
            var medicamento = medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            var prescricoes = prescricaoService.GetAll((uint)id);
            medicamentoModel.PacientesSelecionados = prescricoes.Select(p => p.IdPaciente).ToArray();
            var pacientes = pacienteService.GetAsync();
            medicamentoModel.Pacientes = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MedicamentoViewModel medicamentoModel)
        {
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
                prescricaoService.DeleteByMedicamento((uint)id);

                foreach (var idPaciente in medicamentoModel.PacientesSelecionados!)
                {
                    prescricaoService.Create(new Prescricao
                    {
                        IdMedicamento = (uint)id,
                        IdPaciente = idPaciente
                    });
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicamentoController/Delete/5
        public ActionResult Delete(int id)
        {
            var medicamento = medicamentoService.Get((uint)id);
            var medicamentoModel = mapper.Map<MedicamentoViewModel>(medicamento);
            var prescricoes = prescricaoService.GetAll((uint)id);
            medicamentoModel.Pacientes = prescricoes
                                            .Select(p => mapper.Map<PacienteDto>(p.IdPacienteNavigation))
                                            .ToList();
            return View(medicamentoModel);
        }

        // POST: MedicamentoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, MedicamentoViewModel _)
        {
            prescricaoService.DeleteByMedicamento((uint)id);
            medicamentoService.Delete((uint)id);
            return RedirectToAction(nameof(Index));
        }
    }
}
