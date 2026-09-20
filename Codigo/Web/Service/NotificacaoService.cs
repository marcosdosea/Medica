using Core.Service;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly ILogger<NotificacaoService> logger;

        public NotificacaoService(ILogger<NotificacaoService> logger)
        {
            this.logger = logger;
        }

        public async Task Enviar(string token, string titulo, string corpo)
        {
            var message = new Message
            {
                Topic = token,
                Notification = new Notification
                {
                    Title = titulo,
                    Body = corpo
                }
            };

            string resposta = await FirebaseMessaging.DefaultInstance.SendAsync(message, dryRun: true);
            logger.LogInformation("Notificação Firebase enviada com sucesso. Resposta: {Resposta}", resposta);
        }
    }
}
