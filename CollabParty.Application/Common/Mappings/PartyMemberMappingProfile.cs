using AutoMapper;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class PartyMemberMappingProfile : Profile
{
    public PartyMemberMappingProfile()
    {
        CreateMap<PartyMember, PartyMemberResponseDto>()
            .ForMember(dest => dest.PartyId, opt => opt.MapFrom(src => src.PartyId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.CurrentLevel, opt => opt.MapFrom(src => src.User.CurrentLevel))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src =>
                src.User.UnlockedAvatars.Where(ua => ua.IsActive).Select(ua => ua).FirstOrDefault()));

        CreateMap<AddPartyMemberResponseDto, PartyMember>().ReverseMap();
    }
}