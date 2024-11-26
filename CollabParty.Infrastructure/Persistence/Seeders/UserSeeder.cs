using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Persistence.Seeders
{
    public class UserSeeder
    {
        public static void Seed(AppDbContext dbContext)
        {
            var random = new Random();
            string[] userDisplayNames =
            {
                "Alex", "Jordan", "Taylor", "Casey", "Riley",
                "Morgan", "Skylar", "Jamie", "Cameron", "Avery"
            };

            var users = new List<ApplicationUser>();
            foreach (var name in userDisplayNames)
            {
                var userLevel = random.Next(1, 101);
                var expForCurrentLevel = CalculateExpForLevel(userLevel - 1); 
                var expForNextLevel = CalculateExpForLevel(userLevel);
                var currencyAmount = random.Next(100, 5001);
                var currentExp = random.Next(expForCurrentLevel, expForNextLevel);
                
                var user = new ApplicationUser
                {
                    UserName = name,
                    Email = $"{name.ToLower()}@test.com",
                    Currency = currencyAmount,
                    CurrentLevel = userLevel,
                    CurrentExp = currentExp,
                    ExpToNextLevel = expForNextLevel,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                };

                users.Add(user);
            }

            if (!dbContext.Users.Any())
            {
                dbContext.Users.AddRange(users);
                dbContext.SaveChanges();

                SeedUserAvatars(dbContext, users);
            }
        }

        private static void SeedUserAvatars(AppDbContext dbContext, List<ApplicationUser> users)
        {
            var userAvatars = new List<UserAvatar>();
            var random = new Random();

            var avatars = dbContext.Avatars.ToList();

            foreach (var user in users)
            {
                var unlockedAvatars = avatars.Where(a => a.UnlockLevel <= user.CurrentLevel).ToList();
                if (unlockedAvatars.Any())
                {
                    var activeAvatar = unlockedAvatars[random.Next(unlockedAvatars.Count)];

                    foreach (var avatar in unlockedAvatars)
                    {
                        userAvatars.Add(new UserAvatar
                        {
                            UserId = user.Id,
                            AvatarId = avatar.Id,
                            UnlockedAt = DateTime.UtcNow,
                            IsActive = avatar.Id == activeAvatar.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            dbContext.UserAvatars.AddRange(userAvatars);
            dbContext.SaveChanges();
        }

        private static int CalculateExpForLevel(int level)
        {
            int baseExp = 100;
            return baseExp * level;
        }
    }
}
