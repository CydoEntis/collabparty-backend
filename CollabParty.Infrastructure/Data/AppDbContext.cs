using CollabParty.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollabParty.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Avatar> Avatars { get; set; }
    public DbSet<Party> Parties { get; set; }
    public DbSet<Quest> Quests { get; set; }
    public DbSet<QuestStep> QuestSteps { get; set; }
    public DbSet<UserAvatar> UserAvatars { get; set; }
    public DbSet<UserQuest> UserQuests { get; set; }
    public DbSet<UserQuestStep> UserQuestSteps { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserAvatar>()
            .HasKey(ua => new { ua.UserId, ua.AvatarId });

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        builder.Entity<UserQuest>()
            .HasKey(uq => new { uq.UserId, uq.QuestId });
    }
}