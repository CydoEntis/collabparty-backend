using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using System.Linq;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public class PartyMemberSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        var random = new Random();
        var users = dbContext.Users.ToList();
        var parties = dbContext.Parties.ToList();

        using (var transaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                foreach (var party in parties)
                {
                    var existingMembers = dbContext.PartyMembers.Where(pm => pm.PartyId == party.Id).ToList();

                    if (existingMembers.Count == 0)
                    {
                        int numberOfMembers = random.Next(2, 6);

                        var availableUsers = users.Where(u => !existingMembers.Select(pm => pm.UserId).Contains(u.Id))
                            .OrderBy(u => random.Next())
                            .Take(numberOfMembers)
                            .ToList();

                        for (int i = 0; i < availableUsers.Count; i++)
                        {
                            var user = availableUsers[i];
                            UserRole role =
                                (i == 0) ? UserRole.Captain : UserRole.Member;

                            // Generate a random date within the current month and before today
                            var today = DateTime.UtcNow.Date;
                            var startOfMonth = new DateTime(today.Year, today.Month, 1);
                            int randomDay = random.Next(0, (today - startOfMonth).Days);

                            var partyMember = new PartyMember
                            {
                                UserId = user.Id,
                                PartyId = party.Id,
                                Role = role,
                                JoinedAt = startOfMonth.AddDays(randomDay)
                            };

                            dbContext.PartyMembers.Add(partyMember);
                        }
                    }
                }

                dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("An error occurred while seeding party members.", ex);
            }
        }
    }
}