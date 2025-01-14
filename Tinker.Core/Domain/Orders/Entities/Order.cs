using Tinker.Core.Domain.Checkout.ValueObjects;
using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Domain.Orders.Events;
using Tinker.Core.Domain.Orders.ValueObjects;

namespace Tinker.Core.Domain.Orders.Entities;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(CustomerId customerId, IEnumerable<OrderItem> items)
    {
        var order = new Order
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

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        AddDomainEvent(new OrderStatusChangedEvent(Id, Status));
    }
}