using System.ComponentModel.DataAnnotations.Schema;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

public class Quest
{
    public int Id { get; set; }

    [ForeignKey("Party")]
    public int PartyId { get; set; }
    public Party Party { get; set; }

    public string CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public PriorityLevelOption PriorityLevel { get; set; }

    public int GoldReward { get; set; }
    public int ExpReward { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime CompletedAt { get; set; }

    public string? CompletedById { get; set; }  
    public ApplicationUser CompletedBy { get; set; }  

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow;

    public ICollection<QuestAssignment> QuestAssignments { get; set; }

    public ICollection<QuestStep> QuestSteps { get; set; }

    public ICollection<QuestComment> QuestComments { get; set; }
    public ICollection<QuestFile> QuestFiles { get; set; }
}


