using Tinker.Shared.DTOs.Extras;
using Tinker.Shared.DTOs.Orders;
using Tinker.Shared.DTOs.Payments;

namespace Tinker.Core.Services.Orders.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetOrders();
    Task<OrderDto?> GetOrderById(int                      id);
    Task ProcessOrder(OrderDto                            orderDto);
    Task<OrderResult> ProcessSale(OrderDto                order);
    Task<PaymentResult> ProcessPayment(PaymentDto         payment);
    Task<IEnumerable<OrderDto>> GetSalesHistory(DateRange range);
    Task<OrderStatusResult> UpdateStatus(int              orderId, OrderStatus status);
    Task UpdateOrderStatus(int                            orderId, string      status);
}