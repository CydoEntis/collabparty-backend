using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Dtos.QuestFiles;
using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Domain.Enums;

namespace CollabParty.Application.Common.Dtos.Quest;

public class QuestDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public PriorityLevelOption PriorityLevel { get; set; }

    public int GoldReward { get; set; }
    public int ExpReward { get; set; }
    public string CreatedById { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CompletedAt { get; set; }
    public string CompletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow;
    public List<PartyMemberResponseDto> PartyMembers { get; set; }
    public List<PartyMemberResponseDto> AssignedMembers { get; set; }
    public int TotalPartyMembers { get; set; }
    public ICollection<QuestStepResponseDto> QuestSteps { get; set; }
    public ICollection<QuestCommentResponseDto> QuestComments { get; set; }
    public ICollection<QuestFilesResponseDto> QuestFiles { get; set; }
}