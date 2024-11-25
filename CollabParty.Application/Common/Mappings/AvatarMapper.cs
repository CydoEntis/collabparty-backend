using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public static class AvatarMapper
{
    public static UserAvatarDto ToUserAvatarDto(Avatar avatar)
    {
        return new UserAvatarDto
        {
            Name = avatar.Name,
            DisplayName = avatar.DisplayName,
            ImageUrl = avatar.ImageUrl
        };
    }
}