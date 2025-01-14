using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Users.ValueObjects;

namespace Tinker.Core.Domain.Users.Event;

public record MfaDisabledEvent(UserId UserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid EventId { get; } = Guid.NewGuid();
}