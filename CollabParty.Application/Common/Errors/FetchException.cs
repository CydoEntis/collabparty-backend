using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class FetchException(string message)
    : ServiceException(StatusCodes.Status500InternalServerError, "Fetch Exception", message);