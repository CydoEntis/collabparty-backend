using System.Security.Claims;

namespace CollabParty.Application.Common.Utility;

public class ClaimsHelper
{
    public static (bool IsValid, string UserId) TryGetUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return (!string.IsNullOrEmpty(userId), userId);
    }
}