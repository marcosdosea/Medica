using Core.Service;

namespace Service
{
    public class DispositivoService : IDispositivoService
    {
        private readonly IAuthService authService;

        public DispositivoService(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<string?> ObterToken(uint idPaciente)
        {
            return await authService.GerarTokenPareamento(idPaciente);
        }
    }
}
