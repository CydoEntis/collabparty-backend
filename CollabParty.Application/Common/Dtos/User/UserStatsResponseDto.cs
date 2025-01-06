using CollabParty.Application.Common.Dtos.Avatar;

namespace CollabParty.Application.Common.Dtos.User;

public class UserStatsResponseDto
{
    public string UserId { get; set; }
    public string Username { get; set; } // Add Username
    public AvatarResponseDto CurrentAvatar { get; set; } // Add CurrentAvatar
    public int CurrentLevel { get; set; }
    public int CurrentExperience { get; set; }
    public int ExperienceToLevelUp { get; set; }
    public int Gold { get; set; }
    public int TotalQuests { get; set; }
    public int CompletedQuests { get; set; }
    public int PastDueQuests { get; set; }
    public int PartiesJoined { get; set; }
    public int UnlockedAvatarCount { get; set; }
    public int TotalAvatarCount { get; set; }
    public int LowQuests { get; set; } // Add LowQuests
    public int MediumQuests { get; set; } // Add MediumQuests
    public int HighQuests { get; set; } // Add HighQuests
    public int CriticalQuests { get; set; } // Add CriticalQuests
}