﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CollabParty.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<PartyMember> PartyMembers { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<QuestAssignment> QuestAssignments { get; set; }
        public DbSet<QuestComment> QuestComments { get; set; }
        public DbSet<QuestFile> QuestFiles { get; set; }
        public DbSet<QuestStep> QuestSteps { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<UnlockedAvatar> UnlockedAvatars { get; set; }
        public DbSet<PartyInvite> PartyInvites { get; set; }

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

            builder.Entity<UnlockedAvatar>()
                .HasKey(ua => new { ua.UserId, ua.AvatarId });

            builder.Entity<UnlockedAvatar>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UnlockedAvatars)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UnlockedAvatar>()
                .HasOne(ua => ua.Avatar)
                .WithMany(a => a.UnlockedAvatars)
                .HasForeignKey(ua => ua.AvatarId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartyMember>()
                .HasKey(up => new { up.UserId, up.PartyId });

            builder.Entity<QuestAssignment>()
                .HasKey(qa => new { qa.QuestId, qa.UserId });

            builder.Entity<PartyMember>()
                .HasOne(pm => pm.Party)
                .WithMany(p => p.PartyMembers)
                .HasForeignKey(pm => pm.PartyId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete when Party is deleted

            builder.Entity<PartyMember>()
                .HasOne(pm => pm.User)
                .WithMany()
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // No delete cascade for PartyMember -> Party

            builder.Entity<Quest>()
                .HasOne(q => q.Party)
                .WithMany(p => p.Quests)
                .HasForeignKey(q => q.PartyId)
                .OnDelete(DeleteBehavior.Cascade); // Delete Quests when Party is deleted

            builder.Entity<Quest>()
                .HasOne(q => q.Party)
                .WithMany(p => p.Quests)
                .HasForeignKey(q => q.PartyId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete Party when Quest is deleted

            builder.Entity<PartyMember>()
                .HasOne(pm => pm.Party)
                .WithMany(p => p.PartyMembers)
                .HasForeignKey(pm => pm.PartyId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete PartyMembers when a Quest is deleted

            builder.Entity<Quest>()
                .HasMany(q => q.QuestAssignments)
                .WithOne(qa => qa.Quest)
                .HasForeignKey(qa => qa.QuestId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete QuestAssignments when Quest is deleted

            // Restrict delete when a QuestAssignment is deleted (Quest will not be deleted)
            builder.Entity<QuestAssignment>()
                .HasOne(qa => qa.Quest)
                .WithMany(q => q.QuestAssignments)
                .HasForeignKey(qa => qa.QuestId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent Quest deletion when a QuestAssignment is deleted

            builder.Entity<QuestAssignment>()
                .HasOne(qa => qa.User)
                .WithMany()
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior
                    .Restrict); // Prevents a User from being deleted when a Quest Assignment is deleted.

            builder.Entity<QuestComment>()
                .HasOne(qc => qc.Quest)
                .WithMany(q => q.QuestComments)
                .HasForeignKey(qc => qc.QuestId)
                .OnDelete(DeleteBehavior
                    .Restrict); // Prevents a Quest From being deleted when Quest Comment is deleted.

            builder.Entity<QuestFile>()
                .HasOne(qc => qc.Quest)
                .WithMany(q => q.QuestFiles)
                .HasForeignKey(qc => qc.QuestId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents a Quest from being deleted when a Quest File is Deleted.

            var dateTimeProperties = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in dateTimeProperties)
            {
                if (property.ClrType == typeof(DateTime))
                {
                    var converter = new ValueConverter<DateTime, DateTime>(
                        d => DateTime.SpecifyKind(d, DateTimeKind.Utc),
                        d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
                    );

                    builder
                        .Entity(property.DeclaringEntityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    var converter = new ValueConverter<DateTime?, DateTime?>(
                        d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d,
                        d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d
                    );

                    builder
                        .Entity(property.DeclaringEntityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }
        }
    }
}