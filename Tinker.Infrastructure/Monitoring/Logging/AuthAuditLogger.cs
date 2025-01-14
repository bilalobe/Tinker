using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Monitoring.Logging;

public class AuthAuditLogger(ILogger<AuthAuditLogger> logger) : IAuthAuditLogger
{
    public async Task LogAuthEventAsync(AuthEvent authEvent)
    {
        var auditLog = new AuthAuditLog
        {
            Timestamp = DateTime.UtcNow,
            UserId = authEvent.UserId,
            Action = authEvent.Action,
            Status = authEvent.Status,
            IpAddress = authEvent.IpAddress,
            UserAgent = authEvent.UserAgent
        };

        logger.LogInformation(
            "Auth event: {Action} by user {UserId} from {IpAddress}",
            authEvent.Action, authEvent.UserId, authEvent.IpAddress);

        await _auditStore.SaveAsync(auditLog);
    }
}