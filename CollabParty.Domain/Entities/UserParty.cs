using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollabParty.Domain.Enums;

namespace CollabParty.Domain.Entities
{
    public class UserParty
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Party")]
        public int PartyId { get; set; }
        public Party Party { get; set; }

        [Required]
        [MaxLength(50)]
        public UserRole Role { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}