namespace CollabParty.Domain.Entities;

public class Session
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ApplicationUser User { get; set; }  
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}