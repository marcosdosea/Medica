using Core.Dto.Auth;
using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("associar-dispositivo")]
        public async Task<IActionResult> AssociarDispositivo([FromBody] AssociarDispositivoRequestDto request)
        {
            var response = await authService.AssociarDispositivo(request.TokenPareamento, request.FcmToken);
            return Ok(DefaultGenericResponse<AuthResponseDto>.Success(response, "Dispositivo associado com sucesso."));
        }
    }
}
