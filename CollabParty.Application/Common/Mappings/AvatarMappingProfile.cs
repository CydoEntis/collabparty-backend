using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class AvatarMappingProfile : Profile
{
    public AvatarMappingProfile()
    {
        CreateMap<Avatar, AvatarResponseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));


        CreateMap<UserAvatar, AvatarResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Avatar.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Avatar.Name))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Avatar.DisplayName))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Avatar.ImageUrl));
        
    }
}