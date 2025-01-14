using Tinker.Core.Domain.Checkout.ValueObjects;
using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Domain.Orders.Events;
using Tinker.Core.Domain.Orders.ValueObjects;
using OrderStatus = Tinker.Shared.DTOs.Orders.OrderStatus;

namespace Tinker.Core.Domain.Checkout.Entities;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    private Order()
    {
    } // For EF

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
}