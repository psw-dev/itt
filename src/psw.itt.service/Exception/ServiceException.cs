namespace psw.itt.service.Exception
{
    public class ServiceException : System.Exception
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public new System.Exception InnerException { get; set; }

        public ServiceException(int code, string message, System.Exception innerException)
        {
            StatusCode = code;
            StatusMessage = message;
            InnerException = innerException;

        }
    }
}