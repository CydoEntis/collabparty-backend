using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;

namespace CollabParty.Application.Common.Dtos.Party;

public class PartyDto
{
    public int Id { get; set; }
    public string  Name { get; set; }
    public string  Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PartyMemberResponseDto> PartyMembers { get; set; }
    // public int TotalMembers { get; set; }
    // public int TotalQuests { get; set; }
    // public int CompletedQuests { get; set; }
    // public int ActiveQuests { get; set; }
    // public int PastDueQuests { get; set; }
}