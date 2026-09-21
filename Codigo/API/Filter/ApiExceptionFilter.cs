using Core.Service;
using MedicaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace MedicaAPI.Filter
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is SecurityTokenExpiredException)
            {
                context.Result = new ObjectResult(DefaultGenericResponse.Error(null, "O código de pareamento expirou. Gere um novo QR Code."))
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;
                return;
            }

            if (exception is ServiceException or ArgumentException or InvalidOperationException or SecurityTokenException)
            {
                context.Result = new ObjectResult(DefaultGenericResponse.Error(null, exception.Message))
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
                context.ExceptionHandled = true;
                return;
            }

            context.Result = new ObjectResult(DefaultGenericResponse.Error(null, "Ocorreu um erro interno no servidor."))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
