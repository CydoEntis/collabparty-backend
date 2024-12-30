using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class NotFoundException(string message)
    : ServiceException(StatusCodes.Status404NotFound, ErrorTitles.NotFoundException, message)
{
}