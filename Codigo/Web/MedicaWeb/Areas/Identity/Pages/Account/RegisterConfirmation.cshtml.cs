// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MedicaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;

namespace MedicaWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<Usuario> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            Email = email;
            DisplayConfirmAccountLink = false;

            return Page();
        }

        public async Task<IActionResult> OnPostResendAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email && !u.EmailConfirmed);
            if (user != null)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                var emailDestino = email;
                var linkConfirmacao = HtmlEncoder.Default.Encode(callbackUrl);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _sender.SendEmailAsync(emailDestino, "Confirmação de Conta - Medica",
                            $"Olá!<br><br>" +
                            $"Você solicitou o reenvio da confirmação de conta no <strong>Sistema Medica</strong>.<br>" +
                            $"Para ativar sua conta e começar a cuidar de quem você quer bem, confirme seu endereço de e-mail clicando no botão abaixo:<br><br>" +
                            $"<div style='text-align: center; margin: 30px 0;'>" +
                            $"  <a href='{linkConfirmacao}' style='background-color: #2854d9; color: #ffffff !important; padding: 14px 28px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 15px; display: inline-block;'>" +
                            $"      Confirmar Minha Conta" +
                            $"  </a>" +
                            $"</div>" +
                            $"<p style='color: #6c757d; font-size: 13px; text-align: center; margin-top: 25px;'>Este link é válido por 2 horas. Se você não solicitou, basta ignorar este e-mail.</p>");
                    }
                    catch
                    {
                    }
                });

                TempData["MensagemSucesso"] = "E-mail de confirmação reenviado com sucesso!";
            }

            Email = email;
            DisplayConfirmAccountLink = false;
            return Page();
        }
    }
}
