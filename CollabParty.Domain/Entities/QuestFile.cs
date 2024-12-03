namespace CollabParty.Domain.Entities;

public class QuestFile
{
    public int Id { get; set; }

    // Foreign key to Quest
    public int QuestId { get; set; }
    public Quest Quest { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string FilePath { get; set; }
    public string FileName { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}