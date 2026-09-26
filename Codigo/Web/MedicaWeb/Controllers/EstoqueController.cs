using AutoMapper;
using Core;
using Core.Dto.Estoque;
using Core.Dto.Paciente;
using Core.Helper;
using Core.Helpers;
using Core.Service;
using MedicaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Util;

namespace MedicaWeb.Controllers
{
    [Authorize(Roles = "Administrador, Cuidador")]
    public class EstoqueController : Controller
    {
        private readonly IEstoqueService estoqueService;
        private readonly IPacienteService pacienteService;
        private readonly IMedicamentoService medicamentoService;
        private readonly IMapper mapper;

        public EstoqueController(
            IEstoqueService estoqueService,
            IPacienteService pacienteService,
            IMedicamentoService medicamentoService,
            IMapper mapper)
        {
            this.estoqueService = estoqueService;
            this.pacienteService = pacienteService;
            this.medicamentoService = medicamentoService;
            this.mapper = mapper;
        }

        // GET: EstoqueController/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var idCuidador = User.GetId();
            var pacientes = await pacienteService.GetAll(idCuidador);
            ViewBag.Pacientes = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            ViewBag.Medicamentos = await medicamentoService.GetAll(idCuidador);
            var estoques = await estoqueService.GetAllByCuidador(idCuidador);
            ViewBag.EstoquesExistentes = mapper.Map<IEnumerable<EstoqueItemDto>>(estoques);
            return View(new GerenciarEstoqueViewModel());
        }

        // POST: EstoqueController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GerenciarEstoqueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var idCuidador = User.GetId();
                var pacientes = await pacienteService.GetAll(idCuidador);
                ViewBag.Pacientes = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
                ViewBag.Medicamentos = await medicamentoService.GetAll(idCuidador);
                var estoques = await estoqueService.GetAllByCuidador(idCuidador);
                ViewBag.EstoquesExistentes = mapper.Map<IEnumerable<EstoqueItemDto>>(estoques);
                return View(model);
            }
            var estoque = mapper.Map<Estoque>(model);
            await estoqueService.Create(estoque, model.IdsPacientes);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Create));
        }

        // POST: EstoqueController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GerenciarEstoqueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var idCuidador = User.GetId();
                var pacientes = await pacienteService.GetAll(idCuidador);
                ViewBag.Pacientes = mapper.Map<IEnumerable<PacienteDto>>(pacientes);
                ViewBag.Medicamentos = await medicamentoService.GetAll(idCuidador);
                var estoques = await estoqueService.GetAllByCuidador(idCuidador);
                ViewBag.EstoquesExistentes = mapper.Map<IEnumerable<EstoqueItemDto>>(estoques);
                return View(nameof(Create), model);
            }
            model.Id = id;
            var estoque = mapper.Map<Estoque>(model);
            await estoqueService.Edit(estoque, model.IdsPacientes);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
            return RedirectToAction(nameof(Create));
        }

        // POST: EstoqueController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await estoqueService.Delete(id);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Create));
        }
    }
}
