using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Common.Errors;

public class PermissionException(string message)
    : ServiceException(StatusCodes.Status403Forbidden, "Permission Denied", message)
{
}