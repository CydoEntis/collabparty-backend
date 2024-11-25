using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Member;

public class MemberDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
    public UserAvatarDto Avatar { get; set; }
    public DateTime JoinedAt { get; set; }
}