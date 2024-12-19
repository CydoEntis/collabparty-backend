using AutoMapper;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Mappings;

public class PartyMappingProfile : Profile
{
    public PartyMappingProfile()
    {
        CreateMap<Party, PartyDto>()
            .ForMember(dest => dest.PartyMembers, opt => opt
                .MapFrom(src =>
                    src.PartyMembers.Where(pm => pm.Role == UserRole.Leader)
                        .Union(src.PartyMembers.Where(pm => pm.Role != UserRole.Leader).Take(3))))
            .ForMember(dest => dest.TotalPartyMembers, opt => opt.MapFrom(src => src.PartyMembers.Count))
            .ForMember(dest => dest.TotalQuests, opt => opt.MapFrom(src => src.Quests.Count))
            .ForMember(dest => dest.CompletedQuests,
                opt => opt.MapFrom(src => src.Quests.Count(q => q.IsCompleted)))
            .ForMember(dest => dest.PastDueQuests,
                opt => opt.MapFrom(src => src.Quests.Count(q => q.DueDate < DateTime.UtcNow)));
        CreateMap<CreatePartyDto, Party>();
    }
}