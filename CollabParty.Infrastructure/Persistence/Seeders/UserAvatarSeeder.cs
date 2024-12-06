using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public class UserAvatarSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (!dbContext.UserAvatars.Any()) 
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

            dbContext.UserAvatars.AddRange(userAvatars);
            dbContext.SaveChanges();
        }
    }
}