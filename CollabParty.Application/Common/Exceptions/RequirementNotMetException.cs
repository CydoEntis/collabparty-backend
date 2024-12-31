using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class RequirementNotMetException(string message)
    : ServiceException(StatusCodes.Status400BadRequest, ErrorTitles.RequirementNotMet, message);