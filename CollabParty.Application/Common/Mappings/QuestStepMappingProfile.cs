using AutoMapper;
using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class QuestStepMappingProfile : Profile
{
    public QuestStepMappingProfile()
    {
        CreateMap<QuestStep, QuestStepResponseDto>();
    }
}