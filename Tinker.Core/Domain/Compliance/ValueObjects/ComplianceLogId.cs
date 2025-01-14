namespace Tinker.Core.Domain.Compliance.ValueObjects;

public record ComplianceLogId(Guid Value)
{
    public static ComplianceLogId New()
    {
        return new ComplianceLogId(Guid.NewGuid());
    }
}