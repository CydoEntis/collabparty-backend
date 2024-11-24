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

        // Success response
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

        // Error response for a single field or general error
        public static ApiResponse Error(string field, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Errors = new Dictionary<string, List<string>> { { field, new List<string> { message } } }
            };
        }

        // Error response for multiple messages for a field
        public static ApiResponse Error(string field, IEnumerable<string> messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = "Validation errors occurred.",
                Errors = new Dictionary<string, List<string>> { { field, messages.ToList() } }
            };
        }

        // General error (non-field specific)
        public static ApiResponse GeneralError(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Errors = new Dictionary<string, List<string>> { { "General", new List<string> { message } } }
            };
        }

        // Validation errors for multiple fields
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

        // Adds a single error for a specific field
        public ApiResponse AddError(string field, string error)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = new List<string>();
            }
            Errors[field].Add(error);
            return this;
        }

        // Adds multiple errors for a specific field
        public ApiResponse AddErrors(string field, IEnumerable<string> errors)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = new List<string>();
            }
            Errors[field].AddRange(errors);
            return this;
        }
    }
}
