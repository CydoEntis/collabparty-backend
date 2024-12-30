using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class NotFoundException(string message)
    : ServiceException(StatusCodes.Status404NotFound, "Not found exception", message)
{
}