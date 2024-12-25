using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Domain.Enums;

public class PartyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PartyMemberResponseDto> PartyMembers { get; set; }
    public int TotalPartyMembers { get; set; }
    public int TotalQuests { get; set; }
    public int CompletedQuests { get; set; }
    public int PastDueQuests { get; set; }
    
    public UserRole CurrentUserRole { get; set; }
}