using System.Net;

namespace CollabParty.Application.Common.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
        public object Result { get; set; }
        public string Message { get; set; }
        public static ApiResponse Success(object result = null, string message = "Operation successful", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Result = result,
                Message = message
            };
        }

        public static ApiResponse Error(string message, Dictionary<string, List<string>> errors = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errors ?? new Dictionary<string, List<string>> { { "General", new List<string> { message } } },
                Message = message
            };
        }

        public static ApiResponse ValidationError(Dictionary<string, List<string>> errors, string message = "Validation failed", HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errors,
                Message = message
            };
        }

        public ApiResponse AddError(string key, string error)
        {
            if (!Errors.ContainsKey(key))
            {
                Errors[key] = new List<string>();
            }
            Errors[key].Add(error);
            return this;
        }
    }
}
