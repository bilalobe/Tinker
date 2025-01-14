using Tinker.Shared.DTOs.Orders;

namespace Tinker.Client.Infrastructure.State.Models;

public record OrderStateModel
{
    public bool IsLoading { get; init; }
    public List<OrderDto> Orders { get; init; } = new();
    public decimal TotalSales { get; init; }
    public int OrderCount { get; init; }
    public DateTime LastUpdated { get; init; }
}