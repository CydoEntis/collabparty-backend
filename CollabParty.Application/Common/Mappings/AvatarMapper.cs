using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public static class AvatarMapper
{
    public static AvatarDto ToAvatarDto(Avatar avatar)
    {
        return new AvatarDto
        {
            Name = avatar.Name,
            DisplayName = avatar.DisplayName,
            ImageUrl = avatar.ImageUrl
        };
    }
}