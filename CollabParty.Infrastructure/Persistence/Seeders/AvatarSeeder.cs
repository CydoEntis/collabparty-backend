using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Persistence.Seeders
{
    public class AvatarSeeder
    {
        public static void Seed(AppDbContext dbContext)
        {
            if (!dbContext.Avatars.Any()) 
            {
                dbContext.Avatars.AddRange(
                    new Avatar
                    {
                        Name = "male_a", DisplayName = "Male Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                        ImageUrl = "/Avatars/male_a.png"
                    },
                    new Avatar
                    {
                        Name = "male_b", DisplayName = "Male Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                        ImageUrl = "/Avatars/male_b.png"
                    },
                    new Avatar
                    {
                        Name = "female_a", DisplayName = "Female Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                        ImageUrl = "/Avatars/female_a.png"
                    },
                    new Avatar
                    {
                        Name = "female_b", DisplayName = "Female Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                        ImageUrl = "/Avatars/female_b.png"
                    },
                    new Avatar
                    {
                        Name = "skeleton_a", DisplayName = "Skeleton Soldier", UnlockLevel = 3, Tier = 1,
                        UnlockCost = 200,
                        ImageUrl = "/Avatars/skeleton_a.png"
                    },
                    new Avatar
                    {
                        Name = "skeleton_b", DisplayName = "Skeleton Captain", UnlockLevel = 3, Tier = 1,
                        UnlockCost = 200,
                        ImageUrl = "/Avatars/skeleton_b.png"
                    },
                    new Avatar
                    {
                        Name = "zombie_male", DisplayName = "Male Zombie", UnlockLevel = 5, Tier = 2, UnlockCost = 350,
                        ImageUrl = "/Avatars/zombie_male.png"
                    },
                    new Avatar
                    {
                        Name = "zombie_female", DisplayName = "Female Zombie", UnlockLevel = 5, Tier = 2,
                        UnlockCost = 350,
                        ImageUrl = "/Avatars/zombie_female.png"
                    },
                    new Avatar
                    {
                        Name = "bear", DisplayName = "Bear", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                        ImageUrl = "/Avatars/bear.png"
                    },
                    new Avatar
                    {
                        Name = "gorilla", DisplayName = "Gorilla", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                        ImageUrl = "/Avatars/gorilla.png"
                    },
                    new Avatar
                    {
                        Name = "frog", DisplayName = "Frog", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                        ImageUrl = "/Avatars/frog.png"
                    },
                    new Avatar
                    {
                        Name = "snake", DisplayName = "Snake", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                        ImageUrl = "/Avatars/snake.png"
                    },
                    new Avatar
                    {
                        Name = "medusa", DisplayName = "Medusa", UnlockLevel = 10, Tier = 4, UnlockCost = 750,
                        ImageUrl = "/Avatars/medusa.png"
                    },
                    new Avatar
                    {
                        Name = "knight", DisplayName = "Knight", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                        ImageUrl = "/Avatars/knight.png"
                    },
                    new Avatar
                    {
                        Name = "priest", DisplayName = "Priest", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                        ImageUrl = "/Avatars/priest.png"
                    },
                    new Avatar
                    {
                        Name = "mage", DisplayName = "Mage", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                        ImageUrl = "/Avatars/mage.png"
                    },
                    new Avatar
                    {
                        Name = "archer", DisplayName = "Archer", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                        ImageUrl = "/Avatars/archer.png"
                    },
                    new Avatar
                    {
                        Name = "rogue", DisplayName = "Rogue", UnlockLevel = 15, Tier = 5, UnlockCost = 1200,
                        ImageUrl = "/Avatars/rogue.png"
                    },
                    new Avatar
                    {
                        Name = "merfolk", DisplayName = "Merfolk", UnlockLevel = 16, Tier = 6, UnlockCost = 1500,
                        ImageUrl = "/Avatars/merfolk.png"
                    },
                    new Avatar
                    {
                        Name = "squidman", DisplayName = "Squidman", UnlockLevel = 16, Tier = 6, UnlockCost = 1500,
                        ImageUrl = "/Avatars/squidman.png"
                    },
                    new Avatar
                    {
                        Name = "fishman", DisplayName = "Fishman", UnlockLevel = 16, Tier = 6, UnlockCost = 1500,
                        ImageUrl = "/Avatars/fishman.png"
                    },
                    new Avatar
                    {
                        Name = "mummy", DisplayName = "Mummy", UnlockLevel = 18, Tier = 7, UnlockCost = 2000,
                        ImageUrl = "/Avatars/mummy.png"
                    },
                    new Avatar
                    {
                        Name = "pharaoh", DisplayName = "Pharaoh", UnlockLevel = 18, Tier = 7, UnlockCost = 2000,
                        ImageUrl = "/Avatars/pharaoh.png"
                    },
                    new Avatar
                    {
                        Name = "spider_a", DisplayName = "Spider", UnlockLevel = 18, Tier = 7, UnlockCost = 2000,
                        ImageUrl = "/Avatars/spider_a.png"
                    },
                    new Avatar
                    {
                        Name = "fanatic", DisplayName = "Fanatic", UnlockLevel = 22, Tier = 8, UnlockCost = 2500,
                        ImageUrl = "/Avatars/fanatic.png"
                    },
                    new Avatar
                    {
                        Name = "prince", DisplayName = "Prince", UnlockLevel = 22, Tier = 8, UnlockCost = 2500,
                        ImageUrl = "/Avatars/prince.png"
                    },
                    new Avatar
                    {
                        Name = "occultist", DisplayName = "Occultist", UnlockLevel = 22, Tier = 8, UnlockCost = 2500,
                        ImageUrl = "/Avatars/occultist.png"
                    },
                    new Avatar
                    {
                        Name = "slime", DisplayName = "Slime", UnlockLevel = 28, Tier = 9, UnlockCost = 3000,
                        ImageUrl = "/Avatars/slime.png"
                    },
                    new Avatar
                    {
                        Name = "mimic", DisplayName = "Mimic", UnlockLevel = 28, Tier = 9, UnlockCost = 3000,
                        ImageUrl = "/Avatars/mimic.png"
                    },
                    new Avatar
                    {
                        Name = "ghoul", DisplayName = "Ghoul", UnlockLevel = 28, Tier = 9, UnlockCost = 3000,
                        ImageUrl = "/Avatars/ghoul.png"
                    },
                    new Avatar
                    {
                        Name = "goblin", DisplayName = "Goblin", UnlockLevel = 32, Tier = 10, UnlockCost = 3500,
                        ImageUrl = "/Avatars/goblin.png"
                    },
                    new Avatar
                    {
                        Name = "werewolf_a", DisplayName = "Werewolf Boss", UnlockLevel = 40, Tier = 11,
                        UnlockCost = 4000,
                        ImageUrl = "/Avatars/werewolf_a.png"
                    },
                    new Avatar
                    {
                        Name = "werewolf_b", DisplayName = "Werewolf Warrior", UnlockLevel = 40, Tier = 11,
                        UnlockCost = 4000, ImageUrl = "/Avatars/werewolf_b.png"
                    },
                    new Avatar
                    {
                        Name = "werewolf_c", DisplayName = "Werewolf Chief", UnlockLevel = 40, Tier = 11,
                        UnlockCost = 4000,
                        ImageUrl = "/Avatars/werewolf_c.png"
                    },
                    new Avatar
                    {
                        Name = "male_orc", DisplayName = "Male Orc", UnlockLevel = 50, Tier = 12, UnlockCost = 5000,
                        ImageUrl = "/Avatars/male_orc.png"
                    },
                    new Avatar
                    {
                        Name = "female_orc", DisplayName = "Female Orc", UnlockLevel = 50, Tier = 12,
                        UnlockCost = 5000,
                        ImageUrl = "/Avatars/female_orc.png"
                    },
                    new Avatar
                    {
                        Name = "lich", DisplayName = "Lich", UnlockLevel = 60, Tier = 13, UnlockCost = 6000,
                        ImageUrl = "/Avatars/lich.png"
                    },
                    new Avatar
                    {
                        Name = "witch", DisplayName = "Witch", UnlockLevel = 70, Tier = 14, UnlockCost = 7000,
                        ImageUrl = "/Avatars/witch.png"
                    },
                    new Avatar
                    {
                        Name = "angel", DisplayName = "Angel", UnlockLevel = 70, Tier = 14, UnlockCost = 7000,
                        ImageUrl = "/Avatars/angel.png"
                    },
                    new Avatar
                    {
                        Name = "male_devil", DisplayName = "Male Devil", UnlockLevel = 80, Tier = 15,
                        UnlockCost = 8000,
                        ImageUrl = "/Avatars/male_devil.png"
                    },
                    new Avatar
                    {
                        Name = "female_devil", DisplayName = "Female Devil", UnlockLevel = 80, Tier = 15,
                        UnlockCost = 8000,
                        ImageUrl = "/Avatars/female_devil.png"
                    },
                    new Avatar
                    {
                        Name = "demon_male", DisplayName = "Male Demon", UnlockLevel = 100, Tier = 16,
                        UnlockCost = 10000,
                        ImageUrl = "/Avatars/demon_male.png"
                    },
                    new Avatar
                    {
                        Name = "demon_female", DisplayName = "Female Demon", UnlockLevel = 100, Tier = 16,
                        UnlockCost = 10000, ImageUrl = "/Avatars/demon_female.png"
                    }
                );

                dbContext.SaveChanges();
            }
        }
    }
}
