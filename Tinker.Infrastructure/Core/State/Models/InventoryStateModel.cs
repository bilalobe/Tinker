using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Infrastructure.Core.State.Models;

public class InventoryStateModel
{
    public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    public int LowStockCount { get; set; }
    public int ExpiringCount { get; set; }
    public bool IsLoading { get; set; }
    public string? Error { get; set; }
}