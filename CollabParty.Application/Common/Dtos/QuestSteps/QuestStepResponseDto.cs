namespace CollabParty.Application.Common.Dtos.QuestSteps;

public class QuestStepResponseDto
{
    public int QuestId { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}