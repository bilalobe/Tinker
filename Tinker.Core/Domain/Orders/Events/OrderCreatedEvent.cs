using Tinker.Core.Domain.Common.Interfaces;

namespace Tinker.Core.Domain.Orders.Events;

public record OrderCreatedEvent(Order Order) : IDomainEvent;