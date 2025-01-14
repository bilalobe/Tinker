namespace Tinker.Shared.DTOs.Orders;

public record OrderItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public required string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountPercentage { get; init; }
}