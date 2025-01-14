using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Services.Checkout.Interfaces;

public interface ICheckoutService
{
    Task ProcessCheckout(OrderDto order);
    Task ValidatePayment(string   paymentMethod, decimal amount);
}