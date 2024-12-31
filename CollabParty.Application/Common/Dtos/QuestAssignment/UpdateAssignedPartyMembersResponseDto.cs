namespace CollabParty.Application.Common.Dtos.QuestAssignment;

public class UpdateAssignedPartyMembersResponseDto
{
    public string Message { get; set; }
    public int QuestId { get; set; }
    public bool IsSuccess { get; set; }
}