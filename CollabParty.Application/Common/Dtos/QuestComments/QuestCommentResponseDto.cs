using CollabParty.Application.Common.Dtos.User;

namespace CollabParty.Application.Common.Dtos.QuestComments;

public class QuestCommentResponseDto
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public UpdatePartyMembersRoleDto PartyMember { get; set; }
    public string Content { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}