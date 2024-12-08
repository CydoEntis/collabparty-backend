using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // CreateMap<ApplicationUser, LoginResponseDto>()
        //     .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));

        CreateMap<RegisterRequestDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<LoginRequestDto, ApplicationUser>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<LoginResponseDto, TokenResponseDto>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.Tokens.AccessToken))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.Tokens.RefreshToken));
    }
}