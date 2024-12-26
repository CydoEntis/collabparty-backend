using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities;

public class QuestComment
{
    public int Id { get; set; }

    [ForeignKey("QuestId")]
    public int QuestId { get; set; }
    public Quest Quest { get; set; }

    [ForeignKey("UserId")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string Content { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
