using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using FluentValidation.AspNetCore;

namespace CollabParty.Application.Common.Validators
{
    public class CamelCaseValidationInterceptor : IValidatorInterceptor
    {
        public ValidationContext<T> BeforeMvcValidation<T>(ControllerContext controllerContext, ValidationContext<T> context)
        {
            return context;
        }

        public ValidationResult AfterMvcValidation<T>(ControllerContext controllerContext, ValidationContext<T> context, ValidationResult result)
        {
            var camelCasedErrors = result.Errors.Select(error =>
                new ValidationFailure(
                    ToCamelCase(error.PropertyName), 
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

        private string ToCamelCase(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return propertyName;

            return char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }
    }
}