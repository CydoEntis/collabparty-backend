namespace CollabParty.Application.Common.Dtos.User;

public class RemoverUserFromPartyDto
{
    public int PartyId { get; set; }
    public List<string> UserIds { get; set; }
}