using AutoMapper;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class PartyMappingProfile : Profile
{
    public PartyMappingProfile()
    {
        CreateMap<Party, PartyDto>();
    }
}