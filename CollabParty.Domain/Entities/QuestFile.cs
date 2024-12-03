using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities;

public class QuestFile
{
    public int Id { get; set; }

    [ForeignKey("QuestId")]
    public int QuestId { get; set; }
    public Quest Quest { get; set; }

    [ForeignKey("UserId")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string FilePath { get; set; }
    public string FileName { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}