using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserDtoResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.CurrentLevel, opt => opt.MapFrom(src => src.CurrentLevel))
            .ForMember(dest => dest.CurrentExp, opt => opt.MapFrom(src => src.CurrentExp))
            .ForMember(dest => dest.ExpToNextLevel, opt => opt.MapFrom(src => src.ExpToNextLevel))
            .ForMember(dest => dest.Avatar,
                opt => opt.MapFrom(src => src.UnlockedAvatars.FirstOrDefault(ua => ua.IsActive).Avatar));
        CreateMap<ApplicationUser, UpdateUserDetailsResponseDto>();

        CreateMap<ApplicationUser, PartyMemberResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.CurrentLevel, opt => opt.MapFrom(src => src.CurrentLevel))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src =>
                src.UnlockedAvatars
                    .Where(ua => ua.IsActive)
                    .Select(a => new AvatarResponseDto
                    {
                        Id = a.Avatar.Id,
                        Name = a.Avatar.Name,
                        DisplayName = a.Avatar.DisplayName,
                        ImageUrl = a.Avatar.ImageUrl
                    })
                    .FirstOrDefault()));
    }
}