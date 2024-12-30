using Microsoft.AspNetCore.Http;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;

namespace CollabParty.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DuplicateException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, ex.Errors);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, ex.Errors);
            }
            catch (ServiceException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, null);
            }
            catch (UnauthorizedAccessException)
            {
                await HandleExceptionAsync(context, "Unauthorized", StatusCodes.Status401Unauthorized,
                    new List<ErrorField>
                    {
                        new ErrorField
                        {
                            Field = "error",
                            Message = "User does not have permission to access this resource."
                        }
                    });
            }
            catch (Exception)
            {
                await HandleExceptionAsync(context, "Internal Server Error", StatusCodes.Status500InternalServerError,
                    new List<ErrorField>
                    {
                        new ErrorField
                        {
                            Field = "error",
                            Message = "An unexpected error occurred. Please try again later."
                        }
                    });
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, string title, int statusCode,
            List<ErrorField>? errors)
        {
            var errorDictionary = errors?.ToDictionary(e => e.Field, e => e.Message) ?? new Dictionary<string, string>();
            var apiError = ApiResponse<object>.ErrorResponse(title, statusCode, errorDictionary);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(apiError);
        }
    }
}
