using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Member;

public class PartyMemberResponseDto
{
    public int PartyId { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public int CurrentLevel { get; set; }
    public UserRole Role { get; set; }
    public AvatarResponseDto Avatar { get; set; }
}