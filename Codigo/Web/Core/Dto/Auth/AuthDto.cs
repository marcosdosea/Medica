using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Auth
{
    public class AssociarDispositivoRequestDto
    {
        [Required(ErrorMessage = "O token de pareamento é obrigatório.")]
        public string TokenPareamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O FCM Token é obrigatório.")]
        public string FcmToken { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string TokenJwt { get; set; } = string.Empty;
    }
}
