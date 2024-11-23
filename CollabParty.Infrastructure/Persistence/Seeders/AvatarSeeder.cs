using CollabParty.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollabParty.Infrastructure.Persistence.Seeders;

public class AvatarSeeder
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<Avatar>().HasData(
            new Avatar
            {
                Id = 1, Name = "male_a", DisplayName = "Male Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                ImageUrl = "/Avatars/male_a.png"
            },
            new Avatar
            {
                Id = 2, Name = "male_b", DisplayName = "Male Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                ImageUrl = "/Avatars/male_b.png"
            },
            new Avatar
            {
                Id = 3, Name = "female_a", DisplayName = "Female Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                ImageUrl = "/Avatars/female_a.png"
            },
            new Avatar
            {
                Id = 4, Name = "female_b", DisplayName = "Female Peasant", UnlockLevel = 1, Tier = 0, UnlockCost = 0,
                ImageUrl = "/Avatars/female_b.png"
            },
            new Avatar
            {
                Id = 5, Name = "skeleton_a", DisplayName = "Skeleton Soldier", UnlockLevel = 3, Tier = 1,
                UnlockCost = 200,
                ImageUrl = "/Avatars/skeleton_a.png"
            },
            new Avatar
            {
                Id = 6, Name = "skeleton_b", DisplayName = "Skeleton Captain", UnlockLevel = 3, Tier = 1,
                UnlockCost = 200,
                ImageUrl = "/Avatars/skeleton_b.png"
            },
            new Avatar
            {
                Id = 7, Name = "zombie_male", DisplayName = "Male Zombie", UnlockLevel = 5, Tier = 2, UnlockCost = 350,
                ImageUrl = "/Avatars/zombie_male.png"
            },
            new Avatar
            {
                Id = 8, Name = "zombie_female", DisplayName = "Female Zombie", UnlockLevel = 5, Tier = 2,
                UnlockCost = 350,
                ImageUrl = "/Avatars/zombie_female.png"
            },
            new Avatar
            {
                Id = 9, Name = "bear", DisplayName = "Bear", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                ImageUrl = "/Avatars/bear.png"
            },
            new Avatar
            {
                Id = 10, Name = "gorilla", DisplayName = "Gorilla", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                ImageUrl = "/Avatars/gorilla.png"
            },
            new Avatar
            {
                Id = 11, Name = "frog", DisplayName = "Frog", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                ImageUrl = "/Avatars/frog.png"
            },
            new Avatar
            {
                Id = 12, Name = "snake", DisplayName = "Snake", UnlockLevel = 8, Tier = 3, UnlockCost = 500,
                ImageUrl = "/Avatars/snake.png"
            },
            new Avatar
            {
                Id = 13, Name = "medusa", DisplayName = "Medusa", UnlockLevel = 10, Tier = 4, UnlockCost = 750,
                ImageUrl = "/Avatars/medusa.png"
            },
            new Avatar
            {
                Id = 14, Name = "knight", DisplayName = "Knight", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                ImageUrl = "/Avatars/knight.png"
            },
            new Avatar
            {
                Id = 15, Name = "priest", DisplayName = "Priest", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                ImageUrl = "/Avatars/priest.png"
            },
            new Avatar
            {
                Id = 16, Name = "mage", DisplayName = "Mage", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                ImageUrl = "/Avatars/mage.png"
            },
            new Avatar
            {
                Id = 17, Name = "archer", DisplayName = "Archer", UnlockLevel = 12, Tier = 5, UnlockCost = 1000,
                ImageUrl = "/Avatars/archer.png"
            },
            new Avatar
            {
                Id = 18, Name = "rogue", DisplayName = "Rogue", UnlockLevel = 15, Tier = 5, UnlockCost = 1200,
                ImageUrl = "/Avatars/rogue.png"
            },
            new Avatar
            {
                Id = 19, Name = "merfolk", DisplayName = "Merfolk", UnlockLevel = 16, Tier = 6, UnlockCost = 1500,
                ImageUrl = "/Avatars/merfolk.png"
            },
            new Avatar
            {
                Id = 20, Name = "squidman", DisplayName = "Squidman", UnlockLevel = 16, Tier = 6, UnlockCost = 1500,
                ImageUrl = "/Avatars/squidman.png"
            },
            new Avatar
            {
                Id = 21, Name = "fishman", DisplayName = "Fishman", UnlockLevel = 16, Tier = 6, UnlockCost = 1500,
                ImageUrl = "/Avatars/fishman.png"
            },
            new Avatar
            {
                Id = 22, Name = "mummy", DisplayName = "Mummy", UnlockLevel = 18, Tier = 7, UnlockCost = 2000,
                ImageUrl = "/Avatars/mummy.png"
            },
            new Avatar
            {
                Id = 23, Name = "pharaoh", DisplayName = "Pharaoh", UnlockLevel = 18, Tier = 7, UnlockCost = 2000,
                ImageUrl = "/Avatars/pharaoh.png"
            },
            new Avatar
            {
                Id = 24, Name = "spider_a", DisplayName = "Spider", UnlockLevel = 18, Tier = 7, UnlockCost = 2000,
                ImageUrl = "/Avatars/spider_a.png"
            },
            new Avatar
            {
                Id = 25, Name = "fanatic", DisplayName = "Fanatic", UnlockLevel = 22, Tier = 8, UnlockCost = 2500,
                ImageUrl = "/Avatars/fanatic.png"
            },
            new Avatar
            {
                Id = 26, Name = "prince", DisplayName = "Prince", UnlockLevel = 22, Tier = 8, UnlockCost = 2500,
                ImageUrl = "/Avatars/prince.png"
            },
            new Avatar
            {
                Id = 27, Name = "occultist", DisplayName = "Occultist", UnlockLevel = 22, Tier = 8, UnlockCost = 2500,
                ImageUrl = "/Avatars/occultist.png"
            },
            new Avatar
            {
                Id = 28, Name = "slime", DisplayName = "Slime", UnlockLevel = 28, Tier = 9, UnlockCost = 3000,
                ImageUrl = "/Avatars/slime.png"
            },
            new Avatar
            {
                Id = 29, Name = "mimic", DisplayName = "Mimic", UnlockLevel = 28, Tier = 9, UnlockCost = 3000,
                ImageUrl = "/Avatars/mimic.png"
            },
            new Avatar
            {
                Id = 30, Name = "ghoul", DisplayName = "Ghoul", UnlockLevel = 28, Tier = 9, UnlockCost = 3000,
                ImageUrl = "/Avatars/ghoul.png"
            },
            new Avatar
            {
                Id = 31, Name = "goblin", DisplayName = "Goblin", UnlockLevel = 32, Tier = 10, UnlockCost = 3500,
                ImageUrl = "/Avatars/goblin.png"
            },
            new Avatar
            {
                Id = 32, Name = "werewolf_a", DisplayName = "Werewolf Boss", UnlockLevel = 40, Tier = 11,
                UnlockCost = 4000,
                ImageUrl = "/Avatars/werewolf_a.png"
            },
            new Avatar
            {
                Id = 33, Name = "werewolf_b", DisplayName = "Werewolf Warrior", UnlockLevel = 40, Tier = 11,
                UnlockCost = 4000, ImageUrl = "/Avatars/werewolf_b.png"
            },
            new Avatar
            {
                Id = 34, Name = "werewolf_c", DisplayName = "Werewolf Chief", UnlockLevel = 40, Tier = 11,
                UnlockCost = 4000,
                ImageUrl = "/Avatars/werewolf_c.png"
            },
            new Avatar
            {
                Id = 35, Name = "male_orc", DisplayName = "Male Orc", UnlockLevel = 50, Tier = 12, UnlockCost = 5000,
                ImageUrl = "/Avatars/male_orc.png"
            },
            new Avatar
            {
                Id = 36, Name = "female_orc", DisplayName = "Female Orc", UnlockLevel = 50, Tier = 12,
                UnlockCost = 5000,
                ImageUrl = "/Avatars/female_orc.png"
            },
            new Avatar
            {
                Id = 37, Name = "lich", DisplayName = "Lich", UnlockLevel = 60, Tier = 13, UnlockCost = 6000,
                ImageUrl = "/Avatars/lich.png"
            },
            new Avatar
            {
                Id = 38, Name = "witch", DisplayName = "Witch", UnlockLevel = 70, Tier = 14, UnlockCost = 7000,
                ImageUrl = "/Avatars/witch.png"
            },
            new Avatar
            {
                Id = 39, Name = "angel", DisplayName = "Angel", UnlockLevel = 70, Tier = 14, UnlockCost = 7000,
                ImageUrl = "/Avatars/angel.png"
            },
            new Avatar
            {
                Id = 40, Name = "male_devil", DisplayName = "Male Devil", UnlockLevel = 80, Tier = 15,
                UnlockCost = 8000,
                ImageUrl = "/Avatars/male_devil.png"
            },
            new Avatar
            {
                Id = 41, Name = "female_devil", DisplayName = "Female Devil", UnlockLevel = 80, Tier = 15,
                UnlockCost = 8000,
                ImageUrl = "/Avatars/female_devil.png"
            },
            new Avatar
            {
                Id = 42, Name = "demon_male", DisplayName = "Male Demon", UnlockLevel = 100, Tier = 16,
                UnlockCost = 10000,
                ImageUrl = "/Avatars/demon_male.png"
            },
            new Avatar
            {
                Id = 43, Name = "demon_female", DisplayName = "Female Demon", UnlockLevel = 100, Tier = 16,
                UnlockCost = 10000, ImageUrl = "/Avatars/demon_female.png"
            }
        );
    }
}