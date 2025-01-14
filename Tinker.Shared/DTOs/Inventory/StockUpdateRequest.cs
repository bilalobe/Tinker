namespace Tinker.Shared.DTOs.Inventory;

public class StockUpdateRequest
{
    public int Quantity { get; }
    public string Operation { get; } = string.Empty;
}