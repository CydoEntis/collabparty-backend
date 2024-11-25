// using AutoMapper;
// using CollabParty.Application.Common.Dtos.Avatar;
// using CollabParty.Application.Common.Dtos.Party;
// using CollabParty.Domain.Entities;
//
// namespace CollabParty.Api.Mappings;
//
// public class PartyMappingProfile : Profile
// {
//     public PartyMappingProfile()
//     {
//         CreateMap<Party, CreatePartyDto>().ReverseMap();
//         CreateMap<Avatar, UserAvatarDto>().ReverseMap();
//
//         CreateMap<Party, PartyDto>()
//             .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src =>
//                 src.UserParties.SelectMany(up => up.User.UserAvatars).Where(ua => ua.IsActive).Select(ua =>
//                     new UserAvatarDto
//                     {
//                         Name = ua.Avatar.Name,
//                         DisplayName = ua.Avatar.DisplayName,
//                         ImageUrl = ua.Avatar.ImageUrl
//                     }).ToList()
//             ));
//     }
// }