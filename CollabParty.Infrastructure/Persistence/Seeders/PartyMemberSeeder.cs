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
                            numberOfMembers = Math.Min(numberOfMembers, users.Count);

                            var selectedUsers = users.OrderBy(u => random.Next()).Take(numberOfMembers).ToList();

                            int leaderIndex = 0;
                            int captainIndex = 1;

                            foreach (var user in selectedUsers)
                            {
                                UserRole role;
                                if (selectedUsers.IndexOf(user) == leaderIndex)
                                    role = UserRole.Leader;
                                else if (selectedUsers.IndexOf(user) == captainIndex || selectedUsers.IndexOf(user) == captainIndex + 1)
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
