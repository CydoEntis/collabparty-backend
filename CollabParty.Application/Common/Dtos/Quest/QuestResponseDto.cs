using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Quest;

public class QuestResponseDto
{  
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public PriorityLevelOption PriorityLevel { get; set; }
    public string[] Steps { get; set; }
    public int[] PartyMembers { get; set; }
    public string CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
    public string CompletedBy { get; set; }
}