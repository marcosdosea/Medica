using AutoMapper;
using Core;
using Core.Dto.Estoque;
using Core.Dto.Paciente;
using Core.Dto.Planejamento;
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
    public class PlanejamentoController : Controller
    {
        private readonly IPlanejamentoService planejamentoService;
        private readonly IPacienteService pacienteService;
        private readonly IEstoqueService estoqueService;
        private readonly INotificacaoService notificacaoService;
        private readonly IMapper mapper;

        public PlanejamentoController(
            IPlanejamentoService planejamentoService,
            IPacienteService pacienteService,
            IEstoqueService estoqueService,
            INotificacaoService notificacaoService,
            IMapper mapper)
        {
            this.planejamentoService = planejamentoService;
            this.pacienteService = pacienteService;
            this.estoqueService = estoqueService;
            this.notificacaoService = notificacaoService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Create(uint? idPaciente = null)
        {
            var idCuidador = User.GetId();
            var pacientesEntidades = await pacienteService.GetAll(idCuidador);
            ViewBag.Pacientes = mapper.Map<IEnumerable<PacienteDto>>(pacientesEntidades);
            var estoques = await estoqueService.GetAllByCuidador(idCuidador);
            ViewBag.MedicamentosEstoque = mapper.Map<IEnumerable<MedicamentoEstoqueDto>>(estoques);
            var planejamentos = await planejamentoService.GetAll(idCuidador);
            var planejamentosAtivos = planejamentos.Where(p => p.Ativo == "S");
            ViewBag.PlanejamentosExistentes = mapper.Map<IEnumerable<PlanejamentoItemDto>>(planejamentosAtivos);
            return View(new PlanejamentoViewModel { IdPaciente = idPaciente ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanejamentoViewModel planejamentoViewModel)
        {
            var planejamentos = mapper.Map<IEnumerable<Planejamento>>(planejamentoViewModel);
            await planejamentoService.Create(planejamentos);
            await notificacaoService.NotificarDispositivosPaciente(planejamentoViewModel.IdPaciente);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.CadastroSucesso);
            return RedirectToAction(nameof(Create), new { idPaciente = planejamentoViewModel.IdPaciente });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, PlanejamentoViewModel planejamentoViewModel)
        {
            planejamentoViewModel.Id = id;
            var planejamentoModel = mapper.Map<Planejamento>(planejamentoViewModel);
            await planejamentoService.Edit(planejamentoModel);
            await notificacaoService.NotificarDispositivosPaciente(planejamentoViewModel.IdPaciente);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.EdicaoSucesso);
            return RedirectToAction(nameof(Create), new { idPaciente = planejamentoViewModel.IdPaciente });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id, uint idPaciente)
        {
            await planejamentoService.Delete(id);
            await notificacaoService.NotificarDispositivosPaciente(idPaciente);
            NotificacaoHelper.AlertaSucesso(TempData, MensagemHelper.DelecaoSucesso);
            return RedirectToAction(nameof(Create), new { idPaciente });
        }
    }
}
