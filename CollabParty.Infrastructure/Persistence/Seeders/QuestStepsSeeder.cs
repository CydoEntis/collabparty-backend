using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

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
            int stepCount = random.Next(2, 6); // 2-5 steps per quest
            for (int i = 0; i < stepCount; i++)
            {
                var isCompleted = random.NextDouble() < 0.5; // 50% chance for the step to be completed
                questSteps.Add(new QuestStep
                {
                    QuestId = quest.Id,
                    Description = $"Step {i + 1} description for quest {quest.Name}",
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = isCompleted ? DateTime.UtcNow.AddDays(-random.Next(1, 30)) : null // Random completion date in the past or null
                });
            }
        }

        dbContext.QuestSteps.AddRange(questSteps);
        dbContext.SaveChanges();
    }
}