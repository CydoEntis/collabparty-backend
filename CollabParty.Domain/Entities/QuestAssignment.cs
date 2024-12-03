using CollabParty.Domain.Entities;

public class QuestAssignment
{
    public int QuestId { get; set; }
    public Quest Quest { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }

    public DateTime AssignedAt { get; set; }
    public bool IsCompleted { get; set; }
}