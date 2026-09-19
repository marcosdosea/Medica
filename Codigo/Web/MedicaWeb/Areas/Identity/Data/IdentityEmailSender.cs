using Email;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MedicaWeb.Areas.Identity.Data
{
    public class IdentityEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string template = EmailTemplate.ConfirmarEmail;

            if (subject.Contains("Senha", StringComparison.OrdinalIgnoreCase) ||
                subject.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                subject.Contains("Reset", StringComparison.OrdinalIgnoreCase))
            {
                template = EmailTemplate.RedefinirSenha;
            }
            else if (subject.Contains("Alterar", StringComparison.OrdinalIgnoreCase) ||
                     subject.Contains("Change", StringComparison.OrdinalIgnoreCase))
            {
                template = EmailTemplate.AlterarEmail;
            }

            var emailModel = new EmailModel
            {
                Assunto = subject,
                Subject = subject,
                To = [email],
                Body = htmlMessage,
                TemplateNome = template
            };

            await EmailService.Enviar(emailModel);
        }
    }
}
