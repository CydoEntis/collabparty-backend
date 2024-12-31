using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class IsRequiredException(string message)
    : ServiceException(StatusCodes.Status400BadRequest, ErrorTitles.RequiredException, message);