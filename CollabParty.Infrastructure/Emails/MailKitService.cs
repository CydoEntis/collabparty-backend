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
        // Create a MimeMessage
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", _email));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        // Create the email body
        var bodyBuilder = new BodyBuilder { TextBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        // Connect to the SMTP server and send the email
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