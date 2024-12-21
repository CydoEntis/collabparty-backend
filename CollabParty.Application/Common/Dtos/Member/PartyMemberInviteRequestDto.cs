namespace CollabParty.Application.Common.Dtos.Member;

public class PartyMemberInviteRequestDto
{
    public int PartyId { get; set; }
    public string InviteeEmail { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
}