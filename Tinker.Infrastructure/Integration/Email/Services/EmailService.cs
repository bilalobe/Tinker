// Integration/Email/Services/EmailService.cs

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Tinker.Infrastructure.Configuration.Groups.Services;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;

namespace Tinker.Infrastructure.Integration.Email.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _settings;

    public EmailService(
        ILogger<EmailService>   logger,
        IOptions<EmailSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Tinker POS", _settings.FromAddress));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.Port, true);
        await client.SendAsync(message);
    }
}