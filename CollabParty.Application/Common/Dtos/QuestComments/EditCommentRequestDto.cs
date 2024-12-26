namespace CollabParty.Application.Common.Dtos.QuestComments;

public class EditCommentRequestDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
}