using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Domain.Entities;

namespace CollabParty.Api.Mappings;

public class PartyMappingProfile : Profile
{
    public PartyMappingProfile()
    {
        CreateMap<Party, PartyDto>().ReverseMap();
        CreateMap<Party, CreatePartyDto>().ReverseMap();
    }
}