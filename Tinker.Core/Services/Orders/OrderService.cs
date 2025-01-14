using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Application.Orders.Commands.CreateCommand;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Domain.Orders.Repositories;
using Tinker.Core.Services.Loyalty.Interfaces;
using Tinker.Core.Services.Orders.Interfaces;
using Tinker.Shared.DTOs.Extras;
using Tinker.Shared.DTOs.Orders;
using Tinker.Shared.DTOs.Payments;

namespace Tinker.Core.Services.Orders;

public class OrderService(
    IOrderProcessor       orderProcessor,
    INotificationService  notificationService,
    ILoyaltyService       loyaltyService,
    ILogger<OrderService> logger,
    IMediator             mediator,
    IOrderRepository      orderRepository)
    : IOrderService
{
    private readonly IOrderProcessor _orderProcessor = orderProcessor;

    public Task<IEnumerable<OrderDto>> GetOrders()
    {
        return _orderProcessor.GetOrders();
    }

    public Task<OrderDto?> GetOrderById(int id)
    {
        return _orderProcessor.GetOrderById(id);
    }

    public async Task ProcessOrder(OrderDto orderDto)
    {
        await _orderProcessor.ProcessOrder(orderDto);
        await loyaltyService.UpdatePoints(orderDto.CustomerId, orderDto.TotalAmount);
    }

    public async Task<OrderResult> ProcessSale(OrderDto order)
    {
        try
        {
            await ProcessOrder(order);
            await notificationService.SendOrderConfirmation(order.Id);

            return new OrderResult
            {
                Success = true,
                OrderId = order.Id,
                Message = "Order processed successfully"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing sale for order");
            return new OrderResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<PaymentResult> ProcessPayment(PaymentDto payment)
    {
        try
        {
            await _orderProcessor.UpdateOrderStatus(payment.OrderId, OrderStatus.Paid);
            await notificationService.SendPaymentConfirmation(payment.OrderId);

            return new PaymentResult
            {
                Success = true,
                Message = "Payment processed successfully"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment");
            return new PaymentResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public Task<OrderStatusResult> UpdateStatus(int orderId, OrderStatus status)
    {
        return _orderProcessor.UpdateOrderStatus(orderId, status);
    }

    public Task UpdateOrderStatus(int orderId, string status)
    {
        return _orderProcessor.UpdateOrderStatus(orderId, Enum.Parse<OrderStatus>(status));
    }

    public Task<IEnumerable<OrderDto>> GetSalesHistory(DateRange range)
    {
        return _orderProcessor.GetSalesHistory(range);
    }

    public async Task CreateOrder(int customerId, List<OrderItemDto> items)
    {
        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = items
        };

        await mediator.Send(command);
    }

    public async Task<OrderDto> CreateOrder(OrderDto orderDto)
    {
        var order = Order.CreateOrder(new CustomerId(orderDto.CustomerId), orderDto.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }));

        await orderRepository.AddAsync(order);

        return order.ToDto();
    }
}