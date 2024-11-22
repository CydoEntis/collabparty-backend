namespace CollabParty.Domain.Entities;

public class Party
{
    public int Id { get; set; }
    public string PartyName { get; set; }
    public List<UserParty> UserParties { get; set; }
    public List<Quest> Quests { get; set; }
}