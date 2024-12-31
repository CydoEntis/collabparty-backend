using Microsoft.AspNetCore.Http;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using UnauthorizedException = SendGrid.Helpers.Errors.Model.UnauthorizedException;

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
            catch (AlreadyExistsException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, ex.Errors);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, ex.Errors);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, new List<ErrorField>
                {
                    new ErrorField
                    {
                        Field = "not_found",
                        Message = ex.Message
                    }
                });
            }
            catch (InvalidTokenException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, new List<ErrorField>
                {
                    new ErrorField
                    {
                        Field = "invalid_token",
                        Message = ex.Message
                    }
                });
            }
            catch (IsRequiredException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, new List<ErrorField>
                {
                    new ErrorField
                    {
                        Field = "required",
                        Message = ex.Message
                    }
                });
            }
            catch (RequirementNotMetException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, new List<ErrorField>
                {
                    new ErrorField
                    {
                        Field = "requirement_not_met",
                        Message = ex.Message
                    }
                });
            }
            catch (OperationException ex)
            {
                await HandleExceptionAsync(context, ex.Title, ex.StatusCode, new List<ErrorField>
                {
                    new ErrorField
                    {
                        Field = "invalid_operation",
                        Message = ex.Message
                    }
                });
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
                            Field = "permission",
                            Message = "User does not have permission to access this resource."
                        }
                    });
            }
            catch (UnauthorizedException)
            {
                await HandleExceptionAsync(context, "Unauthorized", StatusCodes.Status401Unauthorized,
                    new List<ErrorField>
                    {
                        new ErrorField
                        {
                            Field = "permission",
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
            var errorDictionary =
                errors?.ToDictionary(e => e.Field, e => e.Message) ?? new Dictionary<string, string>();
            var apiError = ApiResponse<object>.ErrorResponse(title, statusCode, errorDictionary);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(apiError);
        }
    }
}