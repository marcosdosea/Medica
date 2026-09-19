using System.Collections.Concurrent;
using System.Net.Mail;
using System.Net;
using System.Reflection;

namespace Email
{
    public class EmailService
    {
        private static readonly ConcurrentDictionary<string, string> _cacheTemplates = new(StringComparer.OrdinalIgnoreCase);

        private static string ObterTemplateCss()
        {
            return _cacheTemplates.GetOrAdd("__global_template_css__", _ =>
            {
                var caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "css", "template.css");
                if (File.Exists(caminhoArquivo))
                {
                    return File.ReadAllText(caminhoArquivo);
                }

                var caminhoRelativo = Path.Combine(Directory.GetCurrentDirectory(), "Template", "css", "template.css");
                if (File.Exists(caminhoRelativo))
                {
                    return File.ReadAllText(caminhoRelativo);
                }

                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("Email.Template.css.template.css");
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }

                return string.Empty;
            });
        }

        private static string ObterTemplateHtml(string nomeTemplate)
        {
            if (string.IsNullOrWhiteSpace(nomeTemplate))
            {
                throw new ArgumentException("O nome do template de e-mail é obrigatório.", nameof(nomeTemplate));
            }

            if (!nomeTemplate.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                nomeTemplate += ".html";
            }

            return _cacheTemplates.GetOrAdd(nomeTemplate, nome =>
            {
                string html = string.Empty;
                var caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", nome);
                if (File.Exists(caminhoArquivo))
                {
                    html = File.ReadAllText(caminhoArquivo);
                }
                else
                {
                    var caminhoRelativo = Path.Combine(Directory.GetCurrentDirectory(), "Template", nome);
                    if (File.Exists(caminhoRelativo))
                    {
                        html = File.ReadAllText(caminhoRelativo);
                    }
                    else
                    {
                        var assembly = Assembly.GetExecutingAssembly();
                        using var stream = assembly.GetManifestResourceStream($"Email.Template.{nome}");
                        if (stream != null)
                        {
                            using var reader = new StreamReader(stream);
                            html = reader.ReadToEnd();
                        }
                    }
                }

                if (string.IsNullOrEmpty(html))
                {
                    throw new FileNotFoundException($"O template de e-mail '{nome}' não foi encontrado.");
                }

                if (html.Contains("{CSS}"))
                {
                    html = html.Replace("{CSS}", ObterTemplateCss());
                }

                return html;
            });
        }

        public static Task<bool> Enviar(EmailModel emailModel, string templateNome)
        {
            emailModel.TemplateNome = templateNome;
            return Enviar(emailModel);
        }

        public static async Task<bool> Enviar(EmailModel emailModel)
        {
            if (string.IsNullOrWhiteSpace(emailModel.TemplateNome))
            {
                throw new ArgumentException("O nome do template de e-mail é obrigatório.", nameof(emailModel.TemplateNome));
            }

            if (emailModel.To.Count == 0)
            {
                return false;
            }

            var host = !string.IsNullOrWhiteSpace(emailModel.Host) 
                ? emailModel.Host 
                : Environment.GetEnvironmentVariable("EMAIL_HOST") ?? "smtp.gmail.com";

            var port = emailModel.Port > 0 
                ? emailModel.Port 
                : (int.TryParse(Environment.GetEnvironmentVariable("EMAIL_PORT"), out var p) ? p : 587);

            var userName = !string.IsNullOrWhiteSpace(emailModel.UserName) 
                ? emailModel.UserName 
                : Environment.GetEnvironmentVariable("EMAIL_USERNAME") ?? "sistemamedica@gmail.com";

            var password = !string.IsNullOrWhiteSpace(emailModel.Password) 
                ? emailModel.Password 
                : Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? string.Empty;

            var from = !string.IsNullOrWhiteSpace(emailModel.From) 
                ? emailModel.From 
                : Environment.GetEnvironmentVariable("EMAIL_FROM") ?? userName;

            MailMessage email = new MailMessage();
            email.From = new MailAddress(from, "Sistema Medica");

            foreach (var t in emailModel.To)
            {
                email.To.Add(t);
            }

            email.Subject = !string.IsNullOrWhiteSpace(emailModel.Subject) ? emailModel.Subject : emailModel.Assunto;

            string saudacao = !string.IsNullOrWhiteSpace(emailModel.AddresseeName) 
                ? $"<h3>Olá, {emailModel.AddresseeName}!</h3>" 
                : string.Empty;

            string titulo = !string.IsNullOrWhiteSpace(emailModel.Assunto)
                ? $"<h2>{emailModel.Assunto}</h2>"
                : string.Empty;

            string template = ObterTemplateHtml(emailModel.TemplateNome);
            string corpoFinal = template
                .Replace("{TITULO}", titulo)
                .Replace("{SAUDACAO}", saudacao)
                .Replace("{CONTEUDO}", emailModel.Body);

            email.Body = corpoFinal;
            email.IsBodyHtml = true;

            using (SmtpClient smtp = new(host, port))
            {
                smtp.Credentials = new NetworkCredential(userName, password);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(email);
                return true;
            }
        }
    }
}
