﻿using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;
using System.Linq;
using CollabParty.Domain.Enums;

namespace CollabParty.Infrastructure.Persistence.Seeders
{
    public class PartySeeder
    {
        public static void Seed(AppDbContext dbContext)
        {
            if (!dbContext.Parties.Any())
            {
                var random = new Random();
                string[] partyNames = new string[]
                {
                    "Sprint Planning", "Bug Bash", "Code Review Marathon",
                    "Feature Launch", "API Integration", "Frontend Overhaul",
                    "Backend Optimization", "Database Migration", "Security Audit",
                    "Performance Testing", "UI/UX Enhancement", "CI/CD Pipeline Setup",
                    "Cloud Deployment", "Microservices Architecture", "Refactoring Sprint",
                    "Agile Retrospective", "User Testing", "Code Freeze",
                    "QA Automation", "Version Control Cleanup", "Team Onboarding",
                    "Technical Debt Repayment", "Data Analytics Setup", "Documentation Week",
                    "Innovation Sprint"
                };

                string[] partyDescriptions = new string[]
                {
                    "Organize tasks for the upcoming sprint.", "Fix bugs and improve stability.",
                    "Perform in-depth code reviews across the team.", "Release new features to production.",
                    "Integrate third-party APIs into the application.", "Revamp the frontend for a fresh look.",
                    "Optimize backend performance and response times.", "Migrate data to a new database structure.",
                    "Conduct a thorough security audit of the system.", "Test performance under load and stress.",
                    "Improve the UI/UX based on user feedback.", "Set up automated CI/CD pipelines.",
                    "Deploy the project to the cloud infrastructure.", "Implement microservices for scalability.",
                    "Refactor code for better maintainability.", "Hold a retrospective on the last sprint.",
                    "Conduct user testing sessions.", "Prepare for the code freeze before release.",
                    "Automate quality assurance tests.", "Clean up and organize version control branches.",
                    "Onboard new team members.", "Address and reduce technical debt.",
                    "Set up data analytics for better insights.", "Dedicate time to writing and updating documentation.",
                    "Explore new technologies in an innovation sprint."
                };

                var users = dbContext.ApplicationUsers.ToList();

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var (partyName, description) in partyNames.Zip(partyDescriptions, (n, d) => (n, d)))
                        {
                            var party = new Party
                            {
                                Name = partyName,
                                Description = description,
                                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                                UpdatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30)),
                            };

                            // Select a random user as the leader
                            var leader = users.OrderBy(u => random.Next()).FirstOrDefault();
                            if (leader != null)
                            {
                                party.CreatedById = leader.Id;  // Assign the leader as the creator of the party

                                // First, add the party to the database and get the generated PartyId
                                dbContext.Parties.Add(party);
                                dbContext.SaveChanges();

                                // Add the leader as a party member with the 'Leader' role
                                var partyMember = new PartyMember
                                {
                                    UserId = leader.Id,
                                    PartyId = party.Id,  // This will now have a valid PartyId
                                    Role = UserRole.Leader,
                                    JoinedAt = DateTime.UtcNow
                                };

                                dbContext.PartyMembers.Add(partyMember);
                            }
                        }

                        dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("An error occurred while seeding parties.", ex);
                    }
                }
            }
        }
    }
}
