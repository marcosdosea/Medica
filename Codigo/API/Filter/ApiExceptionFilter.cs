using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace MedicaAPI.Filter
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            logger.LogError(exception, "Erro na API: {Mensagem}", exception.Message);

            if (exception is SecurityTokenExpiredException)
            {
                context.Result = new ObjectResult(DefaultGenericResponse.Error(null, "O código de pareamento expirou. Gere um novo QR Code."))
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;
                return;
            }

            if (exception is SecurityTokenException)
            {
                context.Result = new ObjectResult(DefaultGenericResponse.Error(
                    new { erro = exception.Message },
                    "Código de pareamento inválido."))
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;
                return;
            }

            if (exception is ServiceException or ArgumentException or InvalidOperationException)
            {
                context.Result = new ObjectResult(DefaultGenericResponse.Error(null, exception.Message))
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;
                return;
            }

            var detalhesErro = new
            {
                tipo = exception.GetType().FullName,
                erro = exception.Message,
                inner = exception.InnerException?.Message
            };

            context.Result = new ObjectResult(DefaultGenericResponse.Error(detalhesErro, "Ocorreu um erro interno no servidor."))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
