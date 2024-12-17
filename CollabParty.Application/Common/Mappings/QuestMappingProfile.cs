using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;

namespace CollabParty.Application.Common.Mappings;

public class QuestMappingProfile : Profile
{
    public QuestMappingProfile()
    {
        CreateMap<Quest, CreateQuestRequestDto>().ReverseMap();
        CreateMap<Quest, CreateQuestRequestDto>().ReverseMap();
    }
}