namespace CollabParty.Domain.Entities;

public class Quest
{
    public int Id { get; set; }
    public int PartyId { get; set; }
    public Party Party { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public int Reward { get; set; }
    public List<QuestStep> QuestSteps { get; set; }
    public List<UserQuest> UserQuests { get; set; }
    public bool IsCompleted { get; set; }
}