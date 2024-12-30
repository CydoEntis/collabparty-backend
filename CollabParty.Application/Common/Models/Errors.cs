namespace CollabParty.Application.Common.Errors;

using Microsoft.AspNetCore.Http;

public class ErrorField
{
    public string Field { get; set; }
    public string Message { get; set; }
}
