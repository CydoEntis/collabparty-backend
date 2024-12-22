using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities
{
    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<PartyMember> PartyMembers { get; set; }
        public List<Quest> Quests { get; set; }
    }
}