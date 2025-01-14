using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Orders.Repositories;
using Tinker.Core.Services.Loyalty.Interfaces;
using Tinker.Shared.DTOs.Orders;
using ICheckoutService = Tinker.Core.Services.Checkout.Interfaces.ICheckoutService;

namespace Tinker.Core.Services.Checkout;

public class CheckoutService(
    IOrderRepository         orderRepository,
    ILoyaltyService          loyaltyService,
    ILogger<CheckoutService> logger)
    : ICheckoutService
{
    public async Task ProcessCheckout(OrderDto order)
    {
        logger.LogInformation("Processing checkout for order {OrderId}", order.Id);

        // Validate order
        ValidateOrder(order);

        // Calculate totals
        var subTotal = CalculateSubTotal(order.Items);
        var tax = CalculateTax(subTotal);
        var discount = CalculateDiscount(order.CustomerId, order.Items);

        var totalAmount = subTotal + tax - discount;

        // Update order totals
        order.SubTotal = subTotal;
        order.TaxAmount = tax;
        order.DiscountAmount = discount;
        order.TotalAmount = totalAmount;

        // Save order to database
        var newOrder = new Order
        {
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            Date = DateTime.UtcNow,
            Items = order.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList(),
            SubTotal = subTotal,
            TaxAmount = tax,
            DiscountAmount = discount,
            TotalAmount = totalAmount,
            Status = "Pending"
        };

        await orderRepository.AddAsync(newOrder);

        logger.LogInformation("Checkout processed successfully for order {OrderId}", order.Id);
    }

    public async Task ValidatePayment(string paymentMethod, decimal amount)
    {
        logger.LogInformation("Validating payment of {Amount} via {Method}", amount, paymentMethod);

        // Implement payment validation logic here
        await Task.CompletedTask;
    }

    public async Task RedeemPoints(int customerId, Order order, int pointsToRedeem)
    {
        var discount = CalculateDiscount(pointsToRedeem);
        order.TotalAmount -= discount;

        await loyaltyService.RedeemPoints(customerId, pointsToRedeem, discount);

        await orderRepository.UpdateAsync(order);
    }

    private void ValidateOrder(OrderDto order)
    {
        // Implement order validation logic here
    }

    private decimal CalculateSubTotal(List<OrderItemDto> items)
    {
        return items.Sum(i => i.Quantity * i.UnitPrice);
    }

    private decimal CalculateTax(decimal subTotal)
    {
        // Example: 10% tax rate
        return subTotal * 0.10m;
    }

    private decimal CalculateDiscount(int customerId, List<OrderItemDto> items)
    {
        // Example: Each point is worth $0.05
        var points = loyaltyService.GetPoints(customerId);
        return points * 0.05m;
    }

    private decimal CalculateDiscount(int points)
    {
        // Example: Each point is worth $0.05
        return points * 0.05m;
    }
}