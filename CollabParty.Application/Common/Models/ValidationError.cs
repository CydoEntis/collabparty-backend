namespace CollabParty.Application.Common.Models;

public class ValidationError(string key, string message)
{
    public string Key { get; set; } = key;
    public string Message { get; set; } = message;
}