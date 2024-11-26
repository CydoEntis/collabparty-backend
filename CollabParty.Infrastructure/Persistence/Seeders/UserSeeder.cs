using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public class UserSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (!dbContext.Users.Any()) 
        {
            var random = new Random();
            string[] userDisplayNames =
            {
                "Test", "Alex", "Jordan", "Taylor", "Casey", "Riley",
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

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }
    }

    private static int CalculateExpForLevel(int level)
    {
        int baseExp = 100;
        return baseExp * level;
    }
}