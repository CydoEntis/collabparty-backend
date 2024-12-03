using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Persistence.Seeders;

namespace CollabParty.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Party> Parties { get; set; }

        public DbSet<PartyMember> PartyMembers { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<QuestAssignment> QuestAissignments { get; set; }
        public DbSet<QuestComment> QuestComments { get; set; }
        public DbSet<QuestFile> QuestFiles { get; set; }
        public DbSet<QuestStep> QuestSteps { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<UserAvatar> UserAvatars { get; set; }
        public DbSet<UserQuest> UserQuests { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            
            builder.Entity<UserAvatar>()
                .HasKey(ua => new { ua.UserId, ua.AvatarId });
            
            builder.Entity<UserAvatar>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAvatars) 
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<UserAvatar>()
                .HasOne(ua => ua.Avatar)
                .WithMany(a => a.UserAvatars)  
                .HasForeignKey(ua => ua.AvatarId)
                .OnDelete(DeleteBehavior.NoAction);  
            
            builder.Entity<PartyMember>()
                .HasKey(up => new { up.UserId, up.PartyId });
            
            builder.Entity<UserQuest>()
                .HasKey(uq => new { uq.UserId, uq.QuestId });
        }
    }
}