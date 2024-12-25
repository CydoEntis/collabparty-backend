using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Member;

public class ChangePartyLeaderRequestDto
{
    public string CurrentLeaderId { get; set; } = string.Empty;
    public string NewLeaderId { get; set; } = string.Empty;
    public UserRole NewRoleForPreviousLeader { get; set; } = UserRole.Member;
}