using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using CollabParty.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

public class MailKitService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _email;
    private readonly string _password;

    // Constructor to inject the EmailSettings
    public MailKitService(IConfiguration configuration)
    {
        _smtpHost = configuration["EmailSettings:SmtpHost"];
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
        _email = configuration["EmailSettings:Email"];
        _password = configuration["EmailSettings:Password"];
    }


    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", _email));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body 
        };
        message.Body = bodyBuilder.ToMessageBody();

        using (var smtpClient = new SmtpClient())
        {
            try
            {
                await smtpClient.ConnectAsync(_smtpHost, _smtpPort, false);
                await smtpClient.AuthenticateAsync(_email, _password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Handle errors
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}