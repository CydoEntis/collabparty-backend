using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public static class AvatarMapper
{
    public static AvatarResponseDto ToAvatarDto(Avatar avatar)
    {
        return new AvatarResponseDto
        {
            Name = avatar.Name,
            DisplayName = avatar.DisplayName,
            ImageUrl = avatar.ImageUrl
        };
    }
}