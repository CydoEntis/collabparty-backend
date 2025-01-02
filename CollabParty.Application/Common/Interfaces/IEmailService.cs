namespace CollabParty.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string preview, string toEmail, string subject, string body);
}