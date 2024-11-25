using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Role;

public class RoleChangeDto
{
    public string UserId { get; set; }
    public int PartyId { get; set; }
    public UserRole Role { get; set; }
}