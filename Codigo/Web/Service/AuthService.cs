using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core;
using Core.Dto.Auth;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly MedicaContext context;
        private readonly IConfiguration configuration;

        public AuthService(MedicaContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<string> GerarTokenPareamento(uint idPaciente)
        {
            var paciente = await context.Pacientes.FindAsync(idPaciente)
                ?? throw new ServiceException("Paciente não encontrado no sistema.");

            var secretKey = ObterChaveSecreta();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("IdPaciente", idPaciente.ToString()),
                    new Claim("Tipo", "PAREAMENTO")
                ]),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = configuration["Jwt:Emissor"] ?? "MedicaAPI",
                Audience = configuration["Jwt:Audiencia"] ?? "MedicaMobile"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<AuthResponseDto> AssociarDispositivo(string tokenPareamento, string fcmToken)
        {
            if (string.IsNullOrWhiteSpace(tokenPareamento) || string.IsNullOrWhiteSpace(fcmToken))
            {
                throw new ArgumentException("Token de pareamento e FCM Token são obrigatórios.");
            }

            var secretKey = ObterChaveSecreta();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(tokenPareamento, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var tipo = principal.FindFirst("Tipo")?.Value;
            if (!string.Equals(tipo, "PAREAMENTO", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("O token fornecido não é um token de pareamento válido.");
            }

            var idPacienteStr = principal.FindFirst("IdPaciente")?.Value;
            if (!uint.TryParse(idPacienteStr, out var idPaciente))
            {
                throw new ArgumentException("Token de pareamento sem identificador de paciente válido.");
            }

            var paciente = await context.Pacientes.FindAsync(idPaciente)
                ?? throw new ServiceException("Paciente não encontrado no sistema.");

            var dispositivoExistente = await context.Dispositivopacientes
                .FirstOrDefaultAsync(d => d.FcmToken == fcmToken);

            if (dispositivoExistente != null)
            {
                dispositivoExistente.IdPaciente = idPaciente;
                dispositivoExistente.DataAtualizacao = DateTime.UtcNow;
                context.Dispositivopacientes.Update(dispositivoExistente);
            }
            else
            {
                var novoDispositivo = new Dispositivopaciente
                {
                    IdPaciente = idPaciente,
                    FcmToken = fcmToken,
                    DataAtualizacao = DateTime.UtcNow
                };
                await context.Dispositivopacientes.AddAsync(novoDispositivo);
            }

            await context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim("IdPaciente", paciente.Id.ToString()),
                new Claim(ClaimTypes.Name, paciente.Nome),
                new Claim(ClaimTypes.Role, "Paciente")
            };

            var expiracaoDias = int.TryParse(configuration["Jwt:ExpiracaoDias"], out var dias) ? dias : 365;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(expiracaoDias),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = configuration["Jwt:Emissor"] ?? "MedicaAPI",
                Audience = configuration["Jwt:Audiencia"] ?? "MedicaMobile"
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(jwtToken);

            return new AuthResponseDto
            {
                TokenJwt = jwtString
            };
        }

        private string ObterChaveSecreta()
        {
            return Environment.GetEnvironmentVariable("JWT_CHAVE_SECRETA")
                    ?? configuration["Jwt:ChaveSecreta"]
                    ?? "se_este_token_vazar_em_producao_a_culpa_e_do_estagiario_da_medica_2026!";
        }
    }
}
