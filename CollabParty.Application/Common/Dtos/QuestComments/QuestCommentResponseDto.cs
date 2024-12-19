using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;

namespace CollabParty.Application.Common.Dtos.QuestComments;

public class QuestCommentResponseDto
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public UserDtoResponse PartyMember { get; set; }
    public string Content { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}