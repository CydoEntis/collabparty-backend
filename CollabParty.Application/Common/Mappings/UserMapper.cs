// using CollabParty.Application.Common.Dtos.User;
// using CollabParty.Domain.Entities;
//
// namespace CollabParty.Application.Common.Mappings;
//
// public static class UserMapper
// {
//     public static UserDto ToUserDto(ApplicationUser user)
//     {
//         var activeAvatar = user.UserAvatars
//             .FirstOrDefault(ua => ua.IsActive)?.Avatar;
//
//         return new UserDto
//         {
//             Id = user.Id,
//             Username = user.UserName,
//             CurrentLevel = user.CurrentLevel,
//             CurrentExp = user.CurrentExp,
//             ExpToNextLevel = user.ExpToNextLevel,
//             Avatar = AvatarMapper.ToAvatarDto(activeAvatar),
//         };
//     }
//     
//     public static List<UserDto> ToUserDtoList(IEnumerable<UserParty> userParties)
//     {
//         return userParties
//             .Select(userParty => ToUserDto(userParty.User))
//             .ToList();
//     }
//
//     // public static User FromRemoveUserFromPartyDto(RemoverUserFromPartyDto dto)
//     // {
//     //     return new User
//     //     {
//     //         Id = user.Id,
//     //         Username = user.UserName,
//     //         CurrentLevel = user.CurrentLevel,
//     //         CurrentExp = user.CurrentExp,
//     //         ExpToNextLevel = user.ExpToNextLevel,
//     //         Avatar = AvatarMapper.ToAvatarDto(activeAvatar),
//     //     };
//     // }
//     //
//     // public static List<UserParty> FromRemoveUserDtoList(IEnumerable<RemoverUserFromPartyDto> dtos)
//     // {
//     //     return userParties.Select(user => From)
//     // }
// }