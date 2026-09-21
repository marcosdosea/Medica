using Core.Exceptions;
using Core.Helpers;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using Util;

namespace BibliotecaWeb.Filter
{
    public partial class CustomExceptionFilter : IExceptionFilter
    {
        private readonly IModelMetadataProvider modelMetadataProvider;
        private readonly ITempDataDictionaryFactory tempDataDictionaryFactory;

        public CustomExceptionFilter(
            IModelMetadataProvider modelMetadataProvider,
            ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            this.modelMetadataProvider = modelMetadataProvider;
            this.tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.RouteData.Values.ContainsKey("page"))
            {
                context.Result = new RedirectToPageResult("/Error", new { area = "Identity" });
                context.ExceptionHandled = true;
                return;
            }

            var exception = context.Exception;
            var tempData = tempDataDictionaryFactory.GetTempData(context.HttpContext);
            string errorMessage;

            if (exception is MedicaApiException apiException)
            {
                NotificacaoHelper.AlertaErro(tempData, apiException.Message);
                context.Result = ObterResultadoRedirecionamento(context);
                context.ExceptionHandled = true;
                return;
            }

            if (exception is ServiceException serviceException)
            {
                NotificacaoHelper.AlertaErro(tempData, serviceException.Message);
                context.Result = ObterResultadoRedirecionamento(context);
                context.ExceptionHandled = true;
                return;
            }

            var mySqlException = exception as MySqlException
                  ?? exception.InnerException as MySqlException
                  ?? exception.GetBaseException() as MySqlException;

            if (mySqlException != null)
            {
                if (mySqlException.Number == 1451)
                {
                    errorMessage = "Não é possível excluir este registro, pois ele está vinculado a outros dados no sistema.";
                }
                else if (mySqlException.Number == 1062)
                {
                    errorMessage = FilterHelper.ObterMensagemChaveDuplicada(mySqlException.Message);
                }
                else if (mySqlException.Number == 1048)
                {
                    errorMessage = "Um campo obrigatório não foi preenchido.";
                }
                else if (mySqlException.Number == 1406)
                {
                    errorMessage = "Um campo excedeu o tamanho máximo permitido.";
                }
                else if (mySqlException.Number == 1216 || mySqlException.Number == 1217)
                {
                    var campo = FilterHelper.ExtrairNomeChave(mySqlException.Message);
                    errorMessage = $"Violação de chave estrangeira: operação inválida com {campo}.";
                }
                else if (mySqlException.Number == 1366)
                {
                    errorMessage = "Tipo de dado inválido fornecido para um campo.";
                }
                else
                {
                    errorMessage = "Ocorreu um erro no banco de dados. Por favor entrar em contato com o administrador do sistema.";
                }

                NotificacaoHelper.AlertaErro(tempData, errorMessage);
                context.Result = ObterResultadoRedirecionamento(context);
            }
            else
            {
                errorMessage = "Ocorreu um erro inesperado. Por favor entrar em contato com o administrador do sistema.";

                var result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary(modelMetadataProvider, context.ModelState),
                    TempData = tempData
                };
                result.ViewData["ErrorMessage"] = errorMessage;
                result.ViewData["Exception"] = exception;

                NotificacaoHelper.AlertaErro(tempData, errorMessage);
                context.Result = result;
            }

            context.ExceptionHandled = true;
        }

        private static IActionResult ObterResultadoRedirecionamento(ExceptionContext context)
        {
            var request = context.HttpContext.Request;
            var referer = request.Headers.Referer.ToString();

            if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.RelativeOrAbsolute, out _))
            {
                if (request.HasFormContentType && request.Form.TryGetValue("IdPaciente", out var idPaciente)
                    && !string.IsNullOrWhiteSpace(idPaciente) && idPaciente != "0")
                {
                    var urlBase = referer.Contains('?') ? referer[..referer.IndexOf('?')] : referer;
                    var queryString = referer.Contains('?') ? referer[referer.IndexOf('?')..] : "";
                    var queryParams = QueryHelpers.ParseQuery(queryString);
                    var dictionary = queryParams.ToDictionary(k => k.Key, v => (string?)v.Value.ToString());
                    dictionary["idPaciente"] = idPaciente.ToString();
                    var urlComQuery = QueryHelpers.AddQueryString(urlBase, dictionary);
                    return new RedirectResult(urlComQuery);
                }

                return new RedirectResult(referer);
            }

            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (string.Equals(controllerName, "Paciente", StringComparison.OrdinalIgnoreCase)
                && string.Equals(actionName, "ObterToken", StringComparison.OrdinalIgnoreCase)
                && context.RouteData.Values.TryGetValue("id", out var idVal))
            {
                return new RedirectToActionResult("Details", "Paciente", new { id = idVal });
            }

            var isPostOrDelete = HttpMethods.IsPost(request.Method) || HttpMethods.IsDelete(request.Method);
            if (isPostOrDelete)
            {
                var defaultAction = string.Equals(controllerName, "Planejamento", StringComparison.OrdinalIgnoreCase)
                    ? "Create"
                    : "Index";

                return new RedirectToActionResult(defaultAction, controllerName, null);
            }

            return new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                {
                    ["ErrorMessage"] = "Ocorreu um erro ao processar a solicitação."
                }
            };
        }
    }
}