using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public static class QuestSeeder
{
    private static readonly List<string> TaskNames = new List<string>
    {
        "Implement Authentication Module", "Refactor User Profile Service", "Create REST API Endpoints",
        "Integrate Payment Gateway", "Set Up CI/CD Pipeline", "Fix Bug in User Registration",
        "Write Unit Tests for User Service", "Optimize Database Queries", "Implement User Permissions System",
        "Build Search Functionality", "Create API Documentation", "Refactor Codebase for Performance",
        "Debug Frontend Rendering Issue", "Implement Session Management", "Add Logging to Application",
        "Set Up Database Migrations", "Integrate Third-Party API", "Implement Error Handling Middleware",
        "Optimize Page Load Time", "Create Admin Dashboard", "Set Up User Analytics", "Add Dark Mode Feature",
        "Implement File Upload System", "Create User Activity Tracking", "Fix Mobile Responsiveness Bug",
        "Refactor Database Models", "Write Unit Tests for API", "Implement Pagination in API Responses",
        "Integrate OAuth2 for Login", "Build Data Import/Export Feature", "Create User Notification System",
        "Add Search Index to Database", "Implement Image Compression", "Fix Cross-Browser Compatibility Issues",
        "Implement Rate Limiting", "Set Up Docker for Development", "Create Unit Test Suite for Frontend",
        "Refactor Legacy Code", "Implement Two-Factor Authentication", "Add Support for Multiple Languages",
        "Create API Rate Limiting", "Implement Lazy Loading for Images", "Fix Data Synchronization Issue",
        "Add User Profile Customization", "Integrate Email Notifications",
        "Implement WebSockets for Real-time Updates",
        "Set Up Cloud Storage Integration", "Refactor API Error Handling", "Implement Access Control Lists (ACL)",
        "Build Integration Tests for API", "Implement User Authentication"
    };

    public static void Seed(AppDbContext dbContext)
    {
        var random = new Random();
        var parties = dbContext.Parties.Include(p => p.PartyMembers).ToList();
        if (!parties.Any()) return;

        string[] priorityLevels = { "Low", "Medium", "High", "Critical" };

        foreach (var party in parties)
        {
            if (dbContext.Quests.Any(q => q.PartyId == party.Id)) continue;

            var quests = new List<Quest>();

            for (int i = 0; i < 30; i++) // 30 quests per party
            {
                var questName = TaskNames[random.Next(TaskNames.Count)];
                var questPriority = (PriorityLevelOption)Enum.Parse(
                    typeof(PriorityLevelOption),
                    priorityLevels[random.Next(priorityLevels.Length)]
                );

                var randomMember = party.PartyMembers.OrderBy(_ => random.Next()).FirstOrDefault();

                var twoMonthsFromNow = DateTime.UtcNow.AddMonths(2);
                var randomDay = random.Next(0, 31); 
                var randomDueDate = twoMonthsFromNow.AddDays(randomDay);

                quests.Add(new Quest
                {
                    Name = questName,
                    PartyId = party.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    PriorityLevel = questPriority,
                    ExpReward = CalculateQuestExpReward(questPriority),
                    GoldReward = CalculateQuestGoldReward(questPriority),
                    CreatedById = randomMember?.UserId,
                    DueDate = randomDueDate 
                });
            }

            dbContext.Quests.AddRange(quests);
        }

        dbContext.SaveChanges();
    }

    private static int CalculateQuestExpReward(PriorityLevelOption priorityLevel)
    {
        return priorityLevel switch
        {
            PriorityLevelOption.Low => 50,
            PriorityLevelOption.Medium => 100,
            PriorityLevelOption.High => 200,
            PriorityLevelOption.Critical => 500,
            _ => 0
        };
    }

    private static int CalculateQuestGoldReward(PriorityLevelOption priorityLevel)
    {
        return priorityLevel switch
        {
            PriorityLevelOption.Low => 10,
            PriorityLevelOption.Medium => 20,
            PriorityLevelOption.High => 40,
            PriorityLevelOption.Critical => 100,
            _ => 0
        };
    }
}