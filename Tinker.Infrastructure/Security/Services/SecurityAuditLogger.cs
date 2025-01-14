using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Security.Services;

public class SecurityAuditLogger : ISecurityAuditLogger
{
    private readonly ILogger<SecurityAuditLogger> _logger;

    public SecurityAuditLogger(ILogger<SecurityAuditLogger> logger)
    {
        _logger = logger;
    }

    public async Task LogSecurityEvent(SecurityEvent securityEvent)
    {
        var auditRecord = new SecurityAuditRecord
        {
            Timestamp = DateTime.UtcNow,
            EventType = securityEvent.Type,
            UserId = securityEvent.UserId,
            IpAddress = securityEvent.IpAddress,
            Action = securityEvent.Action,
            Resource = securityEvent.Resource,
            Result = securityEvent.Result,
            AdditionalData = securityEvent.AdditionalData
        };

        _logger.LogInformation("Security Event: {@AuditRecord}", auditRecord);
        await SaveAuditRecord(auditRecord);
    }

    private Task SaveAuditRecord(SecurityAuditRecord auditRecord)
    {
        // Implement saving logic here
        return Task.CompletedTask;
    }
}