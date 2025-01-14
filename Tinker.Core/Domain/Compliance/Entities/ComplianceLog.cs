using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Compliance.ValueObjects;

namespace Tinker.Core.Domain.Compliance.Entities;

public class ComplianceLog(ComplianceLogId id, string description, DateTime timestamp) : AggregateRoot
{
    public ComplianceLogId Id { get; private set; } = id;
    public string Description { get; private set; } = description;
    public DateTime Timestamp { get; private set; } = timestamp;
}