namespace Tinker.Shared.DTOs.Orders;

public record OrderDto
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public required string CustomerName { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public List<OrderItemDto> Items { get; init; } = [];
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public string? PaymentMethod { get; init; }
    public string? PaymentStatus { get; init; }
}