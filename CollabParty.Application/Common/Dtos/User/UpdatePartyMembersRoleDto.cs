using CollabParty.Application.Common.Dtos.Role;

namespace CollabParty.Application.Common.Dtos.User;

public class UpdatePartyMembersRoleDto
{
    public int PartyId { get; set; }
    public List<RoleChangeDto> NewRoles { get; set; }
}