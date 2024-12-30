using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class ResourceModificationException(string message) : ServiceException(StatusCodes.Status500InternalServerError,
    ErrorTitles.ResourceModificationException, message)
{
}