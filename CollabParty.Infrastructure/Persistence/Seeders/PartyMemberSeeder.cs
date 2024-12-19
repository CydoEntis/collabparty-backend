using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using System.Linq;

namespace CollabParty.Infrastructure.Persistence.Seeders
{
    public class PartyMemberSeeder
    {
        public static void Seed(AppDbContext dbContext)
        {
            if (!dbContext.PartyMembers.Any())
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
                            int numberOfMembers = random.Next(1, 11);
                            numberOfMembers =
                                Math.Min(numberOfMembers,
                                    users.Count);


                            var selectedUsers = users.OrderBy(_ => random.Next()).Take(numberOfMembers).ToList();

                            for (int i = 0; i < selectedUsers.Count; i++)
                            {
                                var user = selectedUsers[i];
                                UserRole role;

                                if (i == 0)
                                    role = UserRole.Leader;
                                else if (i == 1 || i == 2)
                                    role = UserRole.Captain;
                                else
                                    role = UserRole.Member;

                                var partyMember = new PartyMember
                                {
                                    UserId = user.Id,
                                    PartyId = party.Id,
                                    Role = role,
                                    JoinedAt = DateTime.UtcNow
                                };

                                dbContext.PartyMembers.Add(partyMember);
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
    }
}