using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<ApplicationUser, LoginDto>()
            .ForMember(dest => dest.User.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.User.Username, opt => opt.MapFrom(src => src.UserName));

        CreateMap<RegisterCredentialsDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<LoginCredentialsDto, ApplicationUser>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<LoginDto, TokenDto>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.Tokens.AccessToken))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.Tokens.RefreshToken));
    }
}