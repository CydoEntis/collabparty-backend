using CollabParty.Application.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class PermissionException(string message)
    : ServiceException(StatusCodes.Status403Forbidden, ErrorTitles.PermissionException, message)
{
}