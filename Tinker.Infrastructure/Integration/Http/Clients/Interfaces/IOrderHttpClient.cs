using Tinker.Shared.DTOs.Orders;

namespace Tinker.Infrastructure.Integration.Http.Clients.Interfaces;

public interface IOrderHttpClient
{
    Task<IEnumerable<OrderDto>> GetOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int    id);
    Task<OrderDto> CreateOrderAsync(OrderDto order);
    Task UpdateOrderStatusAsync(int          orderId, string status);
    Task<byte[]> GetOrderInvoiceAsync(int    orderId);
}