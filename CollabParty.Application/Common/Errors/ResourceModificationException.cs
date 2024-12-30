using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class ResourceModificationException(string message) : ServiceException(StatusCodes.Status500InternalServerError,
    "Resource modification failed", message)
{
}