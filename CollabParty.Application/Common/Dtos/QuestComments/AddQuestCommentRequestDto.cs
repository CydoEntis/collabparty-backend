namespace CollabParty.Application.Common.Dtos.QuestComments;

public class AddQuestCommentRequestDto
{
    public int QuestId { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
}