// Domain/Aggregates/OrderAggregate.cs

using Tinker.Core.Domain.Checkout.ValueObjects;
using Tinker.Core.Domain.Common.Interfaces;
using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Domain.Orders.Events;
using Tinker.Core.Domain.Orders.ValueObjects;
using OrderStatus = Tinker.Shared.DTOs.Orders.OrderStatus;

namespace Tinker.Core.Domain.Checkout.Aggregates;

public class OrderAggregate : AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<OrderItem> _items = new();

    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static OrderAggregate CreateOrder(
        CustomerId             customerId,
        IEnumerable<OrderItem> items)
    {
        var order = new OrderAggregate
        {
            Id = OrderId.New(),
            CustomerId = customerId,
            Status = OrderStatus.New
        };

        order._items.AddRange(items);
        order.CalculateTotal();
        order.AddDomainEvent(new OrderCreatedEvent(order));

        return order;
    }

    public void ApplyDiscount(Money discount)
    {
        TotalAmount -= discount;
        AddDomainEvent(new OrderDiscountAppliedEvent(Id, discount));
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        AddDomainEvent(new OrderStatusChangedEvent(Id, Status));
    }
}