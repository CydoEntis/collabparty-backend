using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Member;

public class AddPartyMemberResponseDto
{
    public int PartyId { get; set; }
    public int PartyMemberId { get; set; }
    public UserRole Role { get; set; }
}