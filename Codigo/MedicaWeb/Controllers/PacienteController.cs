using AutoMapper;
using Core.Dto;
using Core.Helpers;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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

        // GET: Paciente
        public async Task<IActionResult> Index(string searchTerm)
        {
            var pacientes = await pacienteService.GetAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                pacientes = pacientes.Where(p => p.Nome.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewData["SearchTerm"] = searchTerm;
            }

            var pacienteDtos = mapper.Map<IEnumerable<PacienteDto>>(pacientes);

            return View(pacienteDtos);
        }

        // GET: Paciente/Details/5
        public async Task<IActionResult> Details(uint id)
        {
            var paciente = await pacienteService.GetAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            // CORREÇÃO: Mapeando para o DTO que a View de Detalhes espera receber
            var dto = mapper.Map<PacienteDetailsDto>(paciente);
            return View(dto);
        }

        // GET: Paciente/Create
        public IActionResult Create()
        {
            return View(new PacienteDetailsDto());
        }

        // POST: Paciente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteDetailsDto dto)
        {
            if (!ModelState.IsValid)
            {
                NotificacaoHelper.AlertaErro(TempData, "Por favor, corrija os erros do formulário antes de continuar.");
                return View(dto);
            }

            string mensagem = string.Empty;
            HttpStatusCode resultado = (HttpStatusCode)await pacienteService.CreateAsync(dto);

            switch (resultado)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    mensagem = "Paciente <b>Cadastrado</b> com <b>Sucesso</b>!";
                    NotificacaoHelper.AlertaSucesso(TempData, mensagem);
                    return RedirectToAction(nameof(Index));

                case HttpStatusCode.BadRequest:
                    mensagem = "Alerta! <b>CPF</b> ou <b>Cartão do SUS</b> inválido.";
                    NotificacaoHelper.AlertaErro(TempData, mensagem);
                    break;

                case HttpStatusCode.Conflict:
                    mensagem = "Alerta! Já existe um paciente cadastrado com este <b>CPF</b>.";
                    NotificacaoHelper.AlertaErro(TempData, mensagem);
                    break;

                case HttpStatusCode.InternalServerError:
                default:
                    mensagem = "<b>Erro</b>! Desculpe, ocorreu um erro inesperado durante o <b>Cadastro</b> do paciente.";
                    NotificacaoHelper.AlertaErro(TempData, mensagem);
                    break;
            }
            return View(dto);
        }

        // GET: Paciente/Edit/5
        public async Task<IActionResult> Edit(uint id)
        {
            var paciente = await pacienteService.GetAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            // CORREÇÃO: Mapeando para o DTO que a View de Edição espera receber
            var dto = mapper.Map<PacienteDetailsDto>(paciente);
            return View(dto);
        }

        // POST: Paciente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, PacienteDetailsDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await pacienteService.EditAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Delete(uint id)
        {
            // Executa a deleção no banco através do seu serviço
            var sucesso =  pacienteService.DeleteAsync(id);
            if (sucesso == null) 
            {
                // Se der algo errado (ex: restrição de chave), volta para os detalhes com aviso
                TempData["Error"] = "Não foi possível excluir o paciente.";
                return Task.FromResult<IActionResult>(RedirectToAction(nameof(Details), new { id = id }));
            }

            // Se deletou com sucesso, volta para a listagem limpa
            return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));
        }
    }
}