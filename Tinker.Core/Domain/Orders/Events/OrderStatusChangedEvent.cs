using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Orders.ValueObjects;

namespace Tinker.Core.Domain.Orders.Events;

public record OrderStatusChangedEvent(OrderId OrderId, OrderStatus Status) : IDomainEvent;