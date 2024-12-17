using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;

namespace CollabParty.Application.Common.Mappings;

public class QuestMappingProfile : Profile
{
    public QuestMappingProfile()
    {
        CreateMap<CreateQuestRequestDto, Quest>().ForMember(dest => dest.PriorityLevel, opt => opt.MapFrom(src => src.PriorityLevel));
        CreateMap<Quest, QuestResponseDto>().ReverseMap();
    }
}