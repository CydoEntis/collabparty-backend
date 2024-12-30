namespace CollabParty.Application.Common.Errors
{
    public class ServiceException : Exception
    {
        public int StatusCode { get; }
        public string Title { get; }
        public string ErrorMessage { get; }

        public ServiceException(int statusCode, string title, string errorMessage)
            : base(errorMessage)
        {
            StatusCode = statusCode;
            Title = title;
        }
    }
}