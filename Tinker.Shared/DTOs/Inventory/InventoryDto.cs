namespace Tinker.Shared.DTOs.Inventory;

public record InventoryDto
{
    public List<ProductDto> Products { get; init; } = [];
    public decimal TotalValue { get; init; }
    public int LowStockCount { get; init; }
    public int ExpiringCount { get; init; }
}