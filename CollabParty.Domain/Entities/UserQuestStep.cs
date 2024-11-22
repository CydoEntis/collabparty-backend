namespace CollabParty.Domain.Entities;

public class UserQuestStep
{
    public int Id { get; set; }
    public int UserQuestId { get; set; }
    public UserQuest UserQuest { get; set; }
    public int StepId { get; set; }
    public QuestStep QuestStep { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}