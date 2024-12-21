using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Quest;

public class UpdateQuestRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public PriorityLevelOption PriorityLevel { get; set; }
    public string[] AssignedPartyMembers { get; set; }
    public DateTime DueDate { get; set; }
    public List<UpdateQuestStepDto>? Steps { get; set; } 
}