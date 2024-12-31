using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Member;

public class UpdatePartyMemberRoleRequestDto
{
    public string UserId { get; set; }
    public UserRole NewRole { get; set; }
}