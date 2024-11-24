using CollabParty.Application.Common.Dtos.Avatar;

namespace CollabParty.Application.Common.Dtos.Party;

public class PartyDto
{
    public int Id { get; set; }
    public string  PartyName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserAvatarDto> Avatar { get; set; }
    public int TotalMembers { get; set; }
    public int TotalQuests { get; set; }
    public int CompletedQuests { get; set; }
    public int ActiveQuests { get; set; }
    public int PastDueQuests { get; set; }
}