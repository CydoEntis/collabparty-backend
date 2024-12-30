namespace CollabParty.Application.Common.Dtos.Party;

public class UpdatePartyRequestDto
{
        public int PartyId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
}