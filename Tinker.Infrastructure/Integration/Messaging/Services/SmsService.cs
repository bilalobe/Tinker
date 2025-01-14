using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tinker.Infrastructure.Integration.Messaging.Services;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly SmsSettings _settings;

    public SmsService(
        ILogger<SmsService>   logger,
        IOptions<SmsSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendSmsAsync(string to, string message)
    {
        // Implement SMS sending logic here
        _logger.LogInformation("Sending SMS to {To}: {Message}", to, message);
    }
}