using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities
{
    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // The user who created the party
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Party members and quests related to the party
        public List<PartyMember> PartyMembers { get; set; }
        public List<Quest> Quests { get; set; }
    }
}