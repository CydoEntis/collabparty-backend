using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public static class QuestStepsSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        var random = new Random();
        var quests = dbContext.Quests.ToList();
        if (!quests.Any()) return;

        var questSteps = new List<QuestStep>();

        foreach (var quest in quests)
        {
            // Skip seeding if steps already exist for this quest
            if (dbContext.QuestSteps.Any(qs => qs.QuestId == quest.Id)) continue;

            int stepCount = random.Next(2, 6); // 2-5 steps per quest
            for (int i = 0; i < stepCount; i++)
            {
                var isCompleted = random.NextDouble() < 0.5;
                questSteps.Add(new QuestStep
                {
                    QuestId = quest.Id,
                    Description = $"Step {i + 1} description for quest {quest.Name}",
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = isCompleted ? DateTime.UtcNow.AddDays(-random.Next(1, 30)) : null,
                    IsCompleted = isCompleted
                });
            }
        }

        dbContext.QuestSteps.AddRange(questSteps);
        dbContext.SaveChanges();
    }
}