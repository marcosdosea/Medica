// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MedicaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace MedicaWeb.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<Usuario> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
            [EmailAddress(ErrorMessage = "Informe um endereço de e-mail válido.")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == Input.Email && u.EmailConfirmed);
                if (user == null)
                {
                    // Não revelar se o usuário não existe ou se não confirmou e-mail por segurança
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, email = Input.Email },
                    protocol: Request.Scheme);

                var emailDestino = Input.Email;
                var linkRedefinicao = HtmlEncoder.Default.Encode(callbackUrl);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(
                            emailDestino,
                            "Redefinição de Senha - Medica",
                            $"Olá!<br><br>" +
                            $"Recebemos uma solicitação para redefinir a sua senha no <strong>Sistema Medica</strong>.<br>" +
                            $"Para criar uma nova senha e restabelecer o seu acesso, clique no botão abaixo:<br><br>" +
                            $"<div style='text-align: center; margin: 30px 0;'>" +
                            $"  <a href='{linkRedefinicao}' style='background-color: #2854d9; color: #ffffff !important; padding: 14px 28px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 15px; display: inline-block;'>" +
                            $"      Redefinir Minha Senha" +
                            $"  </a>" +
                            $"</div>" +
                            $"<p style='color: #6c757d; font-size: 13px; text-align: center; margin-top: 25px;'>Este link é válido por 2 horas. Se você não solicitou a alteração de senha, ignore este e-mail.</p>");
                    }
                    catch
                    {
                    }
                });

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
