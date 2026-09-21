namespace MedicaAPI.Models
{
    /// <summary>
    /// Representa a estrutura padrão de resposta da API com tipo genérico.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto retornado em Data.</typeparam>
    public class DefaultGenericResponse<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public T? Data { get; set; }

        public DefaultGenericResponse()
        {
        }

        public DefaultGenericResponse(bool sucesso, string mensagem, T? data = default)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        public static DefaultGenericResponse<T> Success(T? data, string mensagem)
        {
            return new DefaultGenericResponse<T>
            {
                Sucesso = true,
                Mensagem = mensagem,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static DefaultGenericResponse<T> Error(T? data, string mensagem)
        {
            return new DefaultGenericResponse<T>
            {
                Sucesso = false,
                Mensagem = mensagem,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static DefaultGenericResponse<T> Error(string mensagem)
        {
            return Error(default, mensagem);
        }
    }

    public class DefaultGenericResponse : DefaultGenericResponse<object?>
    {
        public static DefaultGenericResponse Success(string mensagem)
        {
            return new DefaultGenericResponse
            {
                Sucesso = true,
                Mensagem = mensagem,
                Data = null,
                Timestamp = DateTime.UtcNow
            };
        }

        public static new DefaultGenericResponse Error(string mensagem)
        {
            return new DefaultGenericResponse
            {
                Sucesso = false,
                Mensagem = mensagem,
                Data = null,
                Timestamp = DateTime.UtcNow
            };
        }

        public static new DefaultGenericResponse Error(object? data, string mensagem)
        {
            return new DefaultGenericResponse
            {
                Sucesso = false,
                Mensagem = mensagem,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
