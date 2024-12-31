using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class ConflictException(string message)
    : ServiceException(StatusCodes.Status409Conflict, ErrorTitles.RequirementNotMet, message);