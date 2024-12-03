namespace CollabParty.Domain.Entities;

public class QuestComment
{
    public int Id { get; set; }

    public int QuestId { get; set; }
    public Quest Quest { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string Content { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
