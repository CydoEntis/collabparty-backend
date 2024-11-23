namespace CollabParty.Application.Common.Models;

public class ValidationError
{
    public string Field { get; set; }
    public List<string> Messages { get; set; }

    public ValidationError(string field, IEnumerable<string> messages)
    {
        Field = field;
        Messages = messages.ToList();
    }

    public ValidationError(string field, string message)
    {
        Field = field;
        Messages = new List<string> { message };
    }

    public void AddMessage(string message)
    {
        Messages.Add(message);
    }
}