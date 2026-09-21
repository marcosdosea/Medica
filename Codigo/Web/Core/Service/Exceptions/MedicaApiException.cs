using Core.Service;

namespace Core.Exceptions
{
    [Serializable]
    public class MedicaApiException : ServiceException
    {
        public MedicaApiException()
        {
        }

        public MedicaApiException(string? message) : base(message)
        {
        }

        public MedicaApiException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
