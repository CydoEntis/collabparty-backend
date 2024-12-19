using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public class UnlockedAvatarSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (!dbContext.UnlockedAvatars.Any()) 
        {
            var userAvatars = new List<UnlockedAvatar>();
            var random = new Random();

            var users = dbContext.ApplicationUsers.ToList();
            var avatars = dbContext.Avatars.ToList();

            foreach (var user in users)
            {
                var unlockedAvatars = avatars.Where(a => a.UnlockLevel <= user.CurrentLevel).ToList();
                if (unlockedAvatars.Any())
                {
                    var activeAvatar = unlockedAvatars[random.Next(unlockedAvatars.Count)];

                    foreach (var avatar in unlockedAvatars)
                    {
                        userAvatars.Add(new UnlockedAvatar
                        {
                            UserId = user.Id,
                            AvatarId = avatar.Id,
                            UnlockedAt = DateTime.UtcNow,
                            IsActive = avatar.Id == activeAvatar.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsUnlocked = true,
                        });
                    }
                }
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.UnlockedAvatars.AddRange(userAvatars);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("An error occurred while seeding unlocked avatars.", ex);
                }
            }
        }
    }
}
