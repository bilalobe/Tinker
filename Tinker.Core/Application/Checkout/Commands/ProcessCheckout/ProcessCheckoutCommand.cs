using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Application.Checkout.Commands.ProcessCheckout;

public class ProcessCheckoutCommand(OrderDto order)
{
    public OrderDto Order { get; } = order;
}