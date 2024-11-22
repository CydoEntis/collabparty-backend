namespace CollabParty.Domain.Entities;

public class UserParty
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int PartyId { get; set; }
    public Party Party { get; set; }
    public string Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}