using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class OperationException(string title, string message)
    : ServiceException(StatusCodes.Status400BadRequest, title, message);