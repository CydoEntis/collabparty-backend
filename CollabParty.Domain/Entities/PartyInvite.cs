namespace CollabParty.Domain.Entities;

public class PartyInvite
{
    public int Id { get; set; }
    public int PartyId { get; set; }
    public string InviterUserId { get; set; }
    public string InviteeEmail { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsValid { get; set; } = true;
}