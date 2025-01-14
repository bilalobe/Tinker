namespace Tinker.Core.Domain.Inventory.Events;

public class StockAlert
{
    public StockAlert(Product product)
    {
        ProductId = product.Id;
        ProductName = product.Name;
        Quantity = product.Quantity;
        MinimumStockLevel = product.MinimumStockLevel;
    }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinimumStockLevel { get; set; }
}