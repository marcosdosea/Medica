using Core.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MedicaWeb.Areas.Identity.Data
{
    public class ApplicationSignInManager : SignInManager<Usuario>
    {
        private readonly ICuidadorService _cuidadorService;

        public ApplicationSignInManager(
            UserManager<Usuario> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<Usuario> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<Usuario>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<Usuario> confirmation,
            ICuidadorService cuidadorService)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _cuidadorService = cuidadorService;
        }

        public override async Task<ClaimsPrincipal> CreateUserPrincipalAsync(Usuario user)
        {
            var principal = await base.CreateUserPrincipalAsync(user);
            var identity = principal.Identity as ClaimsIdentity;
            var idUser = await _cuidadorService.GetIdByCpf(user.UserName!);

            identity?.AddClaim(new Claim("idUser", idUser.ToString()));
            return principal;
        }
    }
}