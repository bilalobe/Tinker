namespace Tinker.Core.Domain.Inventory.Events;

public class RestockAlert
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductReference { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}