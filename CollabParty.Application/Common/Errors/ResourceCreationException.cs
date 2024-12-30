using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class ResourceCreationException(string message) : ServiceException(StatusCodes.Status500InternalServerError,
    "Resource modification failed", message)
{
}