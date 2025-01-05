using System.ComponentModel.DataAnnotations.Schema;
using CollabParty.Domain.Entities;

public class QuestAssignment
{
    [ForeignKey("QuestId")]
    public int QuestId { get; set; }
    public Quest Quest { get; set; }

    [ForeignKey("UserId")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public DateTime AssignedAt { get; set; }
}