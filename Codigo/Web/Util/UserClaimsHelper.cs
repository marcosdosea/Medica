using System.Security.Claims;

namespace Util
{
    public static class UserClaimsHelper
    {
        /// <summary>
        /// Obtém o ID do usuário (cuidador) a partir da Claim 'idUser'.
        /// Lança InvalidOperationException caso não exista ou o formato seja inválido.
        /// </summary>
        public static uint GetId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst("idUser")?.Value
                ?? throw new InvalidOperationException("Identificador do usuário não encontrado na sessão.");

            if (!uint.TryParse(idClaim, out var id))
            {
                throw new InvalidOperationException($"O identificador de usuário '{idClaim}' não é um formato numérico válido.");
            }

            return id;
        }

        /// <summary>
        /// Obtém o CPF/UserName registrado no Cookie de autenticação.
        /// </summary>
        public static string GetCpf(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value
                ?? throw new InvalidOperationException("CPF do usuário não encontrado na sessão.");
        }

        /// <summary>
        /// Obtém o e-mail do usuário logado.
        /// </summary>
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new InvalidOperationException("E-mail do usuário não encontrado na sessão.");
        }
    }
}