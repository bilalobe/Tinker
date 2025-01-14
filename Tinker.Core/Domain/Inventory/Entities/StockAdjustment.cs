using Tinker.Core.Domain.Inventory.ValueObjects;

namespace Tinker.Core.Domain.Inventory.Entities;

public class StockAdjustment : Entity
{
    public StockAdjustment(int productId, int quantity, StockOperation operation)
    {
        ProductId = productId;
        Quantity = quantity;
        Operation = operation;
    }

    public StockAdjustment()
    {
        throw new NotImplementedException();
    }

    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public StockOperation Operation { get; private set; }
}