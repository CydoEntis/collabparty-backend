namespace CollabParty.Application.Common.Models;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Expiry { get; set; }
}