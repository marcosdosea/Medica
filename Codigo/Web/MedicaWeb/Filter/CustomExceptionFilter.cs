using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MySql.Data.MySqlClient;
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
            var exception = context.Exception;
            var tempData = tempDataDictionaryFactory.GetTempData(context.HttpContext);
            string errorMessage;

            if (exception is MySqlException mySqlException)
            {
                if (mySqlException.Number == 1451)
                {
                    errorMessage = "Não é possível excluir este registro, pois ele está vinculado a outros dados no sistema.";
                }
                else if (mySqlException.Number == 1062)
                {
                    var campo = FilterHelper.ExtrairNomeChave(mySqlException.Message);
                    errorMessage = $"Já existe um registro com este {campo} cadastrado.";
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
                var actionName = context.RouteData.Values["action"]?.ToString() ?? "Index";
                var viewData = new ViewDataDictionary(modelMetadataProvider, context.ModelState);
                if (context.HttpContext.Items.TryGetValue("ActionModel", out var savedModel))
                {
                    viewData.Model = savedModel;
                }

                context.Result = new ViewResult
                {
                    ViewName = actionName,
                    ViewData = viewData,
                    TempData = tempData
                };
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
    }
}