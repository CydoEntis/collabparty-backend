using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities
{
    public class UserQuest
    {
        [Key] public string UserId { get; set; }
        [Key] public int QuestId { get; set; }

        public ApplicationUser User { get; set; }
        public Quest Quest { get; set; }

        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}