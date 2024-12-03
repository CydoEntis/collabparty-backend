using CollabParty.Domain.Enums;

namespace CollabParty.Domain.Entities;

public class PartyMember
{
    public int PartyId { get; set; }
    public Party Party { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public UserRole Role { get; set; }

    public DateTime JoinedAt { get; set; }
}