using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollabParty.Infrastructure.Persistence.Seeders;

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

            if (users.Any()) 
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

                    var selectedUsers = users.OrderBy(u => random.Next()).Take(3).ToList(); 

                    if (selectedUsers.Any()) 
                    {
                        var leader = selectedUsers.First(); 

                        party.CreatedById = leader.Id;

                        dbContext.Parties.Add(party);
                        dbContext.SaveChanges(); 

                        // Now, seed the PartyMembers
                        foreach (var user in selectedUsers)
                        {
                            var role = user == leader ? UserRole.Leader : UserRole.Member;

                            var partyMember = new PartyMember
                            {
                                UserId = user.Id,
                                PartyId = party.Id,
                                Role = role,
                                JoinedAt = DateTime.UtcNow
                            };

                            dbContext.PartyMembers.Add(partyMember);
                        }

                        dbContext.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("No users found for party: " + partyName);
                    }
                }
            }
            else
            {
                Console.WriteLine("No users found in the database.");
            }
        }
    }
}