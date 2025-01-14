using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Shared.DTOs.Reports;

public record ReportDto
{
    public decimal TotalSales { get; init; }
    public int TotalOrders { get; init; }
    public List<ProductDto> TopProducts { get; init; } = new();
    public Dictionary<DateTime, decimal> DailySales { get; init; } = new();
}