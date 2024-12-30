using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class UnauthorizedException(string message)
    : ServiceException(StatusCodes.Status401Unauthorized, ErrorTitles.UnauthorizedException, message);