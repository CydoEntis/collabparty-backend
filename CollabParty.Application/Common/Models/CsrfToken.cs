namespace CollabParty.Application.Common.Models;

public class CsrfToken
{
    public string Token { get; set; }
    public DateTime Expiry { get; set; }
}