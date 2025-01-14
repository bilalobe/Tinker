using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Client.Infrastructure.State.Models;

public record InventoryStateModel
{
    public bool IsLoading { get; init; }
    public List<ProductDto> Products { get; init; } = new();
    public int LowStockCount { get; init; }
    public int ExpiringCount { get; init; }
    public DateTime LastUpdated { get; init; }
}