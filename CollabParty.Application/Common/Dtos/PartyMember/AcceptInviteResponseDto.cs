namespace CollabParty.Application.Common.Dtos.Member;

public class AcceptInviteResponseDto
{
    public string Message { get; set; }
    public int PartyId { get; set; }
    public string PartyName { get; set; }
    public string PartyDescription { get; set; }
}