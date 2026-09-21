using Core.Dto.Auth;

namespace Core.Service
{
    public interface IAuthService
    {
        Task<string> GerarTokenPareamento(uint idPaciente);
        Task<AuthResponseDto> AssociarDispositivo(string tokenPareamento, string fcmToken);
    }
}
