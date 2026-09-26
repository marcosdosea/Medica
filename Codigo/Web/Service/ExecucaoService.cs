using Core;
using Core.Service;
using Core.Enum.Execucao;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ExecucaoService : IExecucaoService
    {
        private readonly MedicaContext context;

        public ExecucaoService(MedicaContext context)
        {
            this.context = context;
        }

        public async Task<uint> Create(Execucao execucao)
        {
            var planejamento = await context.Planejamentos
                .Include(p => p.IdMedicamentoNavigation)
                .FirstOrDefaultAsync(p => p.Id == execucao.IdPlanejamento)
                ?? throw new ServiceException("Planejamento não encontrado.");

            if (execucao.DataConfirmacao == default)
            {
                DateTime dataBase = DateTime.Today;
                DateTime dataPlanejada = Enumerable.Range(0, 7)
                    .Select(i => dataBase.AddDays(-i))
                    .FirstOrDefault(d => d >= planejamento.DataInicio.Date && IsDiaPlanejado(planejamento.DiaSemana, d.DayOfWeek));

                execucao.DataConfirmacao = dataPlanejada != default ? dataPlanejada : dataBase;
            }
            else
            {
                if (execucao.DataConfirmacao.Date > DateTime.Today)
                {
                    throw new ServiceException("Não é possível registrar uma execução para uma data futura.");
                }

                if (execucao.DataConfirmacao.Date < planejamento.DataInicio.Date)
                {
                    throw new ServiceException("Não é possível registrar uma execução antes do início do tratamento.");
                }
            }

            if (execucao.HoraConfirmacao == null)
            {
                execucao.Status = nameof(Status.FALHA);
            }
            else
            {
                DateTime dataConfirmacao = execucao.DataConfirmacao.Date;
                DateTime momentoConfirmacao = dataConfirmacao + execucao.HoraConfirmacao.Value;
                DateTime doseHoje = dataConfirmacao + planejamento.Hora;
                bool hojeEhPlanejado = IsDiaPlanejado(planejamento.DiaSemana, dataConfirmacao.DayOfWeek) &&
                                       dataConfirmacao >= planejamento.DataInicio.Date &&
                                       (planejamento.DataFim == null || dataConfirmacao <= planejamento.DataFim.Value.Date);

                DateTime momentoPrevisto;
                if (hojeEhPlanejado && (momentoConfirmacao - doseHoje) <= TimeSpan.FromHours(12))
                {
                    momentoPrevisto = doseHoje;
                }
                else
                {
                    DateTime dataAnterior = Enumerable.Range(1, 7)
                        .Select(i => dataConfirmacao.AddDays(-i))
                        .FirstOrDefault(d => d >= planejamento.DataInicio.Date && IsDiaPlanejado(planejamento.DiaSemana, d.DayOfWeek));
                    momentoPrevisto = (dataAnterior != default ? dataAnterior : dataConfirmacao) + planejamento.Hora;
                }

                var atraso = momentoConfirmacao - momentoPrevisto;
                execucao.Status = atraso <= planejamento.IntervaloExecucao
                    ? nameof(Status.SUCESSO)
                    : atraso > TimeSpan.FromHours(24)
                        ? nameof(Status.FALHA)
                        : nameof(Status.ATRASO);
                execucao.DataConfirmacao = momentoPrevisto.Date;

                if (execucao.Status != nameof(Status.FALHA))
                {
                    var estoque = await context.Estoques
                        .FirstOrDefaultAsync(e => e.IdMedicamento == planejamento.IdMedicamento 
                                               && e.IdPacientes.Any(p => p.Id == planejamento.IdPaciente));

                    if (estoque != null)
                    {
                        estoque.Quantidade = Math.Max(0, estoque.Quantidade - planejamento.Dosagem);
                        if (estoque.Quantidade == 0)
                        {
                            estoque.Status = "INSUFICIENTE";
                        }
                        else if (estoque.Quantidade <= estoque.QuantidadeMinima)
                        {
                            estoque.Status = "BAIXO";
                        }
                        else
                        {
                            estoque.Status = "REGULAR";
                        }
                        context.Estoques.Update(estoque);
                    }
                }
            }

            await context.Execucaos.AddAsync(execucao);
            await context.SaveChangesAsync();
            return execucao.Id;
        }

        public async Task<Execucao?> Get(uint id)
        {
            return await context.Execucaos
                .Include(e => e.IdPlanejamentoNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        private static bool IsDiaPlanejado(string diaSemana, DayOfWeek dayOfWeek)
        {
            return !string.IsNullOrWhiteSpace(diaSemana) && diaSemana[(int)dayOfWeek] != 'X';
        }
    }
}