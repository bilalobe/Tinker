// Application/EventHandlers/OrderCreatedEventHandler.cs

using Tinker.Core.Domain.Orders.Events;
using Tinker.Core.Services.Inventory.Interfaces;

namespace Tinker.Core.Application.Orders.EventHandlers.OrderCreated;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;

    public async Task Handle(OrderCreatedEvent @event)
    {
        await _inventoryService.UpdateStock(@event.Order);
        await _notificationService.SendOrderConfirmation(@event.Order.Id);
    }
}