namespace CollabParty.Application.Common.Dtos.Member;

public class RemoveMemberDto
{
    public int PartyId { get; set; }
    public List<string> UserId { get; set; }
}