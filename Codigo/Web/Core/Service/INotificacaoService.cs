namespace Core.Service
{
    public interface INotificacaoService
    {
        Task Enviar(string token, string titulo, string corpo);
    }
}