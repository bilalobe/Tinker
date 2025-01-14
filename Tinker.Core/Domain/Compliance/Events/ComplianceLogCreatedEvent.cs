using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Compliance.Entities;

namespace Tinker.Core.Domain.Compliance.Events;

public record ComplianceLogCreatedEvent(ComplianceLog ComplianceLog) : IDomainEvent;