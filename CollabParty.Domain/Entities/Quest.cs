using System.ComponentModel.DataAnnotations.Schema;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

public class Quest
{
    public int Id { get; set; }

    [ForeignKey("Party")]
    public int PartyId { get; set; }
    public Party Party { get; set; }

    public int CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public PriorityLevel PriorityLevel { get; set; }

    public int GoldReward { get; set; }
    public int ExpReward { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime CompletedAt { get; set; }

    public int? CompletedById { get; set; }  
    public ApplicationUser CompletedBy { get; set; }  

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationship to the QuestAssignments
    public ICollection<QuestAssignment> QuestAssignments { get; set; }

    // Relationship to the QuestSteps
    public ICollection<QuestStep> QuestSteps { get; set; }

    // New relationships to comments and files
    public ICollection<QuestComment> QuestComments { get; set; }
    public ICollection<QuestFile> QuestFiles { get; set; }
}


