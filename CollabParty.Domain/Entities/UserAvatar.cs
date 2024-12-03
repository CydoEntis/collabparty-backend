using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities
{
    public class UserAvatar
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Avatar")]
        public int AvatarId { get; set; }
        public Avatar Avatar { get; set; }

        public bool IsUnlocked { get; set; }
        public DateTime UnlockedAt { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}