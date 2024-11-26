using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using System;
using System.Linq;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public class UserPartySeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (!dbContext.UserParties.Any()) 
        {
            var random = new Random();
            var users = dbContext.Users.ToList();
            var parties = dbContext.Parties.ToList();

            foreach (var party in parties)
            {
                int numberOfMembers = random.Next(1, 11);
                numberOfMembers = Math.Min(numberOfMembers, users.Count);

                var selectedUsers = users.OrderBy(u => random.Next()).Take(numberOfMembers).ToList();

                int leaderIndex = 0;
                int captainIndex = 1;

                for (int i = 0; i < selectedUsers.Count; i++)
                {
                    var user = selectedUsers[i];

                    UserRole role;
                    if (i == leaderIndex)
                        role = UserRole.Leader;
                    else if (i == captainIndex || i == captainIndex + 1)
                        role = UserRole.Captain;
                    else
                        role = UserRole.Member;

                    var userParty = new UserParty
                    {
                        UserId = user.Id,
                        PartyId = party.Id,
                        Role = role,
                        JoinedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    dbContext.UserParties.Add(userParty);
                }

                dbContext.SaveChanges();
            }
        }
    }
}