using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Domain.Orders.Repositories;
using Tinker.Core.Services.Loyalty.Interfaces;
using Tinker.Core.Services.Sales.Interfaces;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Services.Sales;

public class SalesService(
    IOrderRepository      orderRepository,
    ILogger<SalesService> logger,
    ILoyaltyService       loyaltyService,
    INotificationService  notificationService)
    : ISalesService
{
    private readonly ILogger<SalesService> _logger = logger;

    public async Task<OrderDto> ProcessSale(OrderDto orderDto)
    {
        var order = Order.CreateOrder(new CustomerId(orderDto.CustomerId), orderDto.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }));

        await orderRepository.AddAsync(order);

        await loyaltyService.UpdatePoints(orderDto.CustomerId, orderDto.TotalAmount);
        await notificationService.SendOrderConfirmation(order.Id);

        return order.ToDto();
    }
}