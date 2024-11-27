using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CollabParty.Infrastructure.Emails;

public class SendGridEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SendGridEmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new SendGridClient(_emailSettings.SendGridApiKey);
        var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
        var to = new EmailAddress(toEmail);
        var message = MailHelper.CreateSingleEmail(from, to, subject, body, body);
        var response = await client.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to send email: {response.StatusCode}");
        }
    }
}