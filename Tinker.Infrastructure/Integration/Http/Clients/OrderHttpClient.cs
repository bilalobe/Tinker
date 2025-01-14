using Tinker.Infrastructure.Integration.Http.Base;
using Tinker.Infrastructure.Integration.Http.Clients.Interfaces;
using Tinker.Shared.DTOs.Orders;
using ILogger = Serilog.ILogger;

namespace Tinker.Infrastructure.Integration.Http.Clients;

public class OrderHttpClient : BaseHttpClient, IOrderHttpClient
{
    public OrderHttpClient(HttpClient client, ILogger<OrderHttpClient> logger)
        : base(client, (ILogger)logger)
    {
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
    {
        return await GetAsync<IEnumerable<OrderDto>>("api/orders")
               ?? Enumerable.Empty<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        return await GetAsync<OrderDto>($"api/orders/{id}");
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDto order)
    {
        return await PostAsync<OrderDto, OrderDto>("api/orders", order)
               ?? throw new Exception("Failed to create order");
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        await PostAsync<object, object>($"api/orders/{orderId}/status", new { status });
    }

    public async Task<byte[]> GetOrderInvoiceAsync(int orderId)
    {
        return await GetAsync<byte[]>($"api/orders/{orderId}/invoice")
               ?? Array.Empty<byte>();
    }
}