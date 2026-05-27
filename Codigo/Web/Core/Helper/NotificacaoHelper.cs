using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Core.Helpers
{
    public static class NotificacaoHelper
    {
        public static void AlertaSucesso(ITempDataDictionary tempData, string mensagem)
        {
            tempData["Notificacao_Tipo"] = "success";
            tempData["Notificacao_Titulo"] = "Sucesso!";
            tempData["Notificacao_Mensagem"] = mensagem;
        }

        public static void AlertaErro(ITempDataDictionary tempData, string mensagem)
        {
            tempData["Notificacao_Tipo"] = "error";
            tempData["Notificacao_Titulo"] = "Ops! Algo deu errado";
            tempData["Notificacao_Mensagem"] = mensagem;
        }
    }
}