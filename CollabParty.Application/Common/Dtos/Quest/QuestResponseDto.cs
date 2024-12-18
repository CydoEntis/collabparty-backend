using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Quest;

public class QuestResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public PriorityLevelOption PriorityLevel { get; set; }
    public List<PartyMemberResponseDto> PartyMembers { get; set; }
    public int TotalPartyMembers { get; set; }
    public int TotalSteps { get; set; }
    public int CompletedSteps { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
    public string CompletedBy { get; set; }
    public int ExpReward { get; set; }
    public int GoldReward { get; set; }
}