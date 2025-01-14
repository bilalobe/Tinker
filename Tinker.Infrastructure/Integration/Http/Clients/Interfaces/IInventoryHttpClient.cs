using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Infrastructure.Integration.Http.Clients.Interfaces;

public interface IInventoryHttpClient
{
    Task<InventoryDto> GetInventoryDataAsync();
    Task<List<ProductDto>> GetLowStockItemsAsync();
    Task<List<ProductDto>> GetExpiringItemsAsync(int daysThreshold = 90);
    Task<bool> CheckStockAvailabilityAsync(int       productId, int quantity);
    Task UpdateStockLevelAsync(int                   productId, int quantity, string operation);
}