using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class OperationException(string message, string title)
    : ServiceException(StatusCodes.Status400BadRequest, title, message);