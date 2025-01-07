using System.ComponentModel.DataAnnotations;

namespace CollabParty.Domain.Entities
{
    public class Avatar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
        public int UnlockLevel { get; set; }
        public int UnlockCost { get; set; }
        public int Tier { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }= DateTime.UtcNow;
        
        public List<UnlockedAvatar> UnlockedAvatars { get; set; } 
    }
}