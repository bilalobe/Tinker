namespace Tinker.Shared.DTOs.Inventory;

public record ProductDto
{
    public int Id { get; init; }
    public required string Reference { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public bool RequiresRx { get; init; }
    public int MinimumStockLevel { get; init; }
    public string StorageConditions { get; init; } = string.Empty;
}