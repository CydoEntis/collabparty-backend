using CollabParty.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using CollabParty.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace CollabParty.Infrastructure.Persistence.Seeders
{
    public static class UserSeeder
    {
        public static async Task Seed(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
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
                        Gold = currencyAmount,
                        CurrentLevel = userLevel,
                        CurrentExp = currentExp,
                        ExpToNextLevel = expForNextLevel,
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                    };

                    users.Add(user);
                }

                // Begin transaction scope to handle user creation
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        foreach (var user in users)
                        {
                            var result = await userManager.CreateAsync(user, "Test123*");

                            if (!result.Succeeded)
                            {
                                Console.WriteLine(
                                    $"Failed to create user {user.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            }
                        }

                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while seeding users: {ex.Message}");
                        // Transaction will be rolled back automatically if an exception occurs
                    }
                }
            }
        }

        private static int CalculateExpForLevel(int level)
        {
            int baseExp = 100;
            return baseExp * level;
        }
    }
}
