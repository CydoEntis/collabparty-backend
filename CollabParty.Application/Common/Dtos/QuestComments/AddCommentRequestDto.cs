namespace CollabParty.Application.Common.Dtos.QuestComments;

public class AddCommentRequestDto
{
    public int QuestId { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
}