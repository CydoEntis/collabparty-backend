using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class InvalidTokenException : ServiceException
{
    public InvalidTokenException(string errorMessage) : base(StatusCodes.Status401Unauthorized, ErrorTitles.InvalidToken,
        errorMessage)
    {
    }
}