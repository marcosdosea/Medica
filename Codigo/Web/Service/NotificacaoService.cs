using Core;
using Core.Service;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly MedicaContext context;
        private readonly ILogger<NotificacaoService> logger;

        public NotificacaoService(MedicaContext context, ILogger<NotificacaoService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task Enviar(string token, string titulo, string corpo)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                logger.LogWarning("FirebaseApp não está inicializado. Verifique o arquivo firebase-key.json.");
                return;
            }

            var message = new Message
            {
                Fid = token,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = corpo
                },
                Data = new Dictionary<string, string>
                {
                    ["tipo"] = "SINCRONIZAR_PLANEJAMENTOS",
                    ["acao"] = "sincronizar"
                }
            };

            string resposta = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            logger.LogInformation("Notificação Firebase enviada com sucesso. Resposta: {Resposta}", resposta);
        }

        public async Task NotificarDispositivosPaciente(uint idPaciente)
        {
            var tokens = await context.Dispositivopacientes
                .AsNoTracking()
                .Where(d => d.IdPaciente == idPaciente && !string.IsNullOrEmpty(d.FcmToken))
                .Select(d => d.FcmToken)
                .Distinct()
                .ToListAsync();

            foreach (var token in tokens)
            {
                await Enviar(
                    token,
                    "Planejamento alterado",
                    "Novos medicamentos estão chegando na sua agenda.");
            }
        }
    }
}
