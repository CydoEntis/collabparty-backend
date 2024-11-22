namespace CollabParty.Domain.Entities;

public class UserQuest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int QuestId { get; set; }
    public Quest Quest { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<UserQuestStep> UserQuestSteps { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}