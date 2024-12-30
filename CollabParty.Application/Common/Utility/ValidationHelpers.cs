using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Common.Utility;

public static class ValidationHelpers
{
    public static Dictionary<string, List<string>> FormatValidationErrors(IEnumerable<ValidationError> validationErrors)
    {
        var errors = new Dictionary<string, List<string>>();

        foreach (var error in validationErrors)
        {
            if (!errors.ContainsKey(error.Key))
            {
                errors[error.Key] = new List<string>();
            }

            errors[error.Key].Add(error.Message); // Use Add, not AddRange since Message is a single string
        }

        return errors;
    }
}