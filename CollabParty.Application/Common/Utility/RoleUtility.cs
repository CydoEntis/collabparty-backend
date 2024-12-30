using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Utility;

public static class RoleUtility
{
    public static bool IsLeader(PartyMember user)
    {
        return user?.Role == UserRole.Leader;
    }
}