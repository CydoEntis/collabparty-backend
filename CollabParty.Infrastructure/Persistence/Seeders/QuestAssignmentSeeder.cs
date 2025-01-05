using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollabParty.Infrastructure.Persistence.Seeders
{
    public static class QuestAssignmentSeeder
    {
        public static void Seed(AppDbContext dbContext)
        {
            // Check if there are any quest assignments in the database
            if (dbContext.QuestAssignments.Any()) 
                return;  // Exit if quest assignments already exist

            var random = new Random();
            var quests = dbContext.Quests.Include(q => q.Party.PartyMembers).ToList();

            if (!quests.Any()) return;

            foreach (var quest in quests)
            {
                // Get a random number of members to assign (between 1 and 5)
                int numberOfAssignments = random.Next(1, 6);

                // Select a random set of party members (without repetition)
                var selectedMembers = quest.Party.PartyMembers
                    .OrderBy(_ => random.Next()) // Shuffle the list
                    .Take(numberOfAssignments)  // Take a random number of members
                    .ToList();

                // Assign the selected members to the quest
                foreach (var partyMember in selectedMembers)
                {
                    dbContext.QuestAssignments.Add(new QuestAssignment
                    {
                        QuestId = quest.Id,
                        UserId = partyMember.UserId,
                        AssignedAt = DateTime.UtcNow,
                    });
                }
            }

            dbContext.SaveChanges();
        }
    }
}