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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public List<UnlockedAvatar> UserAvatars { get; set; } 
    }
}