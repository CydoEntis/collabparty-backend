namespace CollabParty.Domain.Entities;

public class UserAvatar
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }  
    public int AvatarId { get; set; }
    public Avatar Avatar { get; set; }  
    public DateTime UnlockedAt { get; set; }
    public bool IsActive { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}