using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public static class MemberMapper
{
    public static MemberDto ToMemberDto(UserParty userParty)
    {
        var activeAvatar = userParty.User.UserAvatars
            .FirstOrDefault(ua => ua.IsActive)?.Avatar;

        return new MemberDto
        {
            Id = userParty.User.Id,
            Username = userParty.User.UserName,
            CurrentLevel = userParty.User.CurrentLevel,
            Role = userParty.Role,
            Avatar = AvatarMapper.ToAvatarDto(activeAvatar),
        };
    }
    
    public static List<MemberDto> ToMemberDtoList(IEnumerable<UserParty> userParties)
    {
        return userParties
            .Select(userParty => ToMemberDto(userParty))
            .ToList();
    }
}