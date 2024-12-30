using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CollabParty.Application.Common.Errors
{
    public class Result<T>
    {
        // The Data from a successful action
        public T? Data { get; }

        // Errors from an unsuccessful action
        public Error Error { get; }
        public bool IsSuccess => Error == null;

        private Result(T? data, Error error)
        {
            Data = data;
            Error = error;
        }

        // Success result with value, Example a User Object.
        public static Result<T> Success(T data)
        {
            return new Result<T>(data, null);
        }

        // Success result with a string message
        public static Result<T> Success(string message)
        {
            return new Result<T>((T)(object)message, null);
        }


        // Failure with a simple error message
        public static Result<T> Failure(string code, string message)
        {
            return new Result<T>(default, new Error(code, message));
        }

        // Failure with error fields, used where errors are associated with specific fields 
        // Example: "password": "Password should be at least 8 characters."
        public static Result<T> Failure(string code, string message, Action<Error> addFieldErrors)
        {
            var error = new Error(code, message);
            addFieldErrors?.Invoke(error);
            return new Result<T>(default, error);
        }

        // Automatically maps the result code to the correct HttpResponse 
        public IActionResult Map()
        {
            if (IsSuccess)
            {
                return new OkObjectResult(this);
            }

            return Error.Code switch
            {
                "VALIDATION_ERROR" => new BadRequestObjectResult(this),
                "CONFLICT_ERROR" => new ConflictObjectResult(this),
                "NOT_FOUND" => new NotFoundObjectResult(this),
                "UNAUTHORIZED" => new UnauthorizedObjectResult(this),
                "FORBIDDEN" => new ObjectResult(this) { StatusCode = 403 },
                _ => new ObjectResult(this) { StatusCode = 500 },
            };
        }
    }
}