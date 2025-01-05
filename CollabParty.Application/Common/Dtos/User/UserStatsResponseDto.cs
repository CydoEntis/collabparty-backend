using CollabParty.Application.Common.Dtos.Avatar;

namespace CollabParty.Application.Common.Dtos.User;

public class UserStatsResponseDto
{
    public string UserId { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentExperience { get; set; }
    public int ExperienceThreshold { get; set; }
    public int ExperienceToLevelUp { get; set; }
    public int Gold { get; set; }
    public int TotalQuests { get; set; }
    public int CompletedQuests { get; set; }
    public int PastDueQuests { get; set; }
    public int MonthlyCompletedQuests { get; set; }
    public int PartiesJoined { get; set; }
    public int UnlockedAvatarCount { get; set; }
    public int TotalAvatarCount { get; set; }
    public Dictionary<DateTime, int> MonthlyCompletedQuestsByDay { get; set; }
}