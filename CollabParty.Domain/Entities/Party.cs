using System.ComponentModel.DataAnnotations;

namespace CollabParty.Domain.Entities
{
    public class Party
    {
        public int Id { get; set; }
        public string PartyName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<UserParty> UserParties { get; set; }
        public List<Quest> Quests { get; set; }
    }
}