using CollabParty.Application.Common.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Application.Common.Validators.Helpers
{
    public class ValidationHelper
    {
        public IActionResult HandleValidation(IEnumerable<ValidationFailure> validationErrors)
        {
            var errorDictionary = ConvertValidationErrorsToDictionary(validationErrors);

            var apiError = ApiResponse<object>.ErrorResponse(
                "Validation Exception",
                StatusCodes.Status400BadRequest,
                errorDictionary);

            return new BadRequestObjectResult(apiError);
        }

        private Dictionary<string, string> ConvertValidationErrorsToDictionary(
            IEnumerable<ValidationFailure> validationErrors)
        {
            return validationErrors.ToDictionary(
                error => ToCamelCase(error.PropertyName),
                error => error.ErrorMessage
            );
        }

        private string ToCamelCase(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return propertyName;
            return char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }
    }
}