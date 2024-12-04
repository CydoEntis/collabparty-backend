using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using FluentValidation.AspNetCore;

namespace CollabParty.Application.Common.Validators;
public class CamelCaseValidationInterceptor : IValidatorInterceptor
{
    public ValidationContext<T> BeforeMvcValidation<T>(ControllerContext controllerContext, ValidationContext<T> context)
    {
        return context;
    }

    public ValidationResult AfterMvcValidation<T>(ControllerContext controllerContext, ValidationContext<T> context, ValidationResult result)
    {
        // Convert property names to camelCase for all validation errors
        var camelCasedErrors = result.Errors.Select(error =>
            new ValidationFailure(
                JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName), // Convert to camelCase
                error.ErrorMessage,
                error.AttemptedValue)
            {
                ErrorCode = error.ErrorCode,
                CustomState = error.CustomState,
                Severity = error.Severity
            }).ToList();

        return new ValidationResult(camelCasedErrors);
    }

    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
    {
        return commonContext;
    }

    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
    {
        return result;
    }
}
