using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Utility;

public static class RoleUtility
{
    public static bool IsLeader(PartyMember user)
    {
        return user?.Role == UserRole.Leader;
    }


    public static bool IsLeaderOrCaptain(PartyMember user)
    {
        return user?.Role is UserRole.Leader or UserRole.Captain;
    }
}