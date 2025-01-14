using Tinker.Infrastructure.Integration.Http.Base;
using Tinker.Infrastructure.Integration.Http.Clients.Interfaces;
using Tinker.Shared.DTOs.Inventory;
using ILogger = Serilog.ILogger;

namespace Tinker.Infrastructure.Integration.Http.Clients;

public class InventoryHttpClient : BaseHttpClient, IInventoryHttpClient
{
    public InventoryHttpClient(HttpClient client, ILogger<InventoryHttpClient> logger)
        : base(client, (ILogger)logger)
    {
    }

    public async Task<InventoryDto> GetInventoryDataAsync()
    {
        return await GetAsync<InventoryDto>("/api/inventory")
               ?? throw new Exception("Failed to load inventory data");
    }

    public async Task<List<ProductDto>> GetLowStockItemsAsync()
    {
        return await GetAsync<List<ProductDto>>("/api/inventory/low-stock")
               ?? new List<ProductDto>();
    }

    public async Task<List<ProductDto>> GetExpiringItemsAsync(int daysThreshold = 90)
    {
        return await GetAsync<List<ProductDto>>($"/api/inventory/expiring/{daysThreshold}")
               ?? new List<ProductDto>();
    }

    public async Task<bool> CheckStockAvailabilityAsync(int productId, int quantity)
    {
        return await GetAsync<bool>($"/api/inventory/check-stock/{productId}/{quantity}");
    }

    public async Task UpdateStockLevelAsync(int productId, int quantity, string operation)
    {
        await PostAsync<object, object>("/api/inventory/update-stock", new
        {
            ProductId = productId,
            Quantity = quantity,
            Operation = operation
        });
    }
}