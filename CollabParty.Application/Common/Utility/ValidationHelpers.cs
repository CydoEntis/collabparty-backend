using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Common.Utility;

public static class ValidationHelpers
{
    public static Dictionary<string, List<string>> FormatValidationErrors(IEnumerable<ValidationError> validationErrors)
    {
        var errors = new Dictionary<string, List<string>>();

        foreach (var error in validationErrors)
        {
            if (!errors.ContainsKey(error.Field))
            {
                errors[error.Field] = new List<string>();
            }

            errors[error.Field].AddRange(error.Messages);
        }

        return errors;
    }
}