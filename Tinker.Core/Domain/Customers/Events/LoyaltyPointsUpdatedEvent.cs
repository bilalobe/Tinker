using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Customers.ValueObjects;

namespace Tinker.Core.Domain.Customers.Events;

public record LoyaltyPointsUpdatedEvent(CustomerId CustomerId, int Points) : IDomainEvent;