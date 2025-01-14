using Tinker.Core.Domain.Checkout.ValueObjects;
using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Inventory.ValueObjects;

namespace Tinker.Core.Domain.Products.Aggregates;

public class Product : AggregateRoot
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public StockLevel Quantity { get; private set; }

    public void UpdateStock(int quantity, StockOperation operation)
    {
        var newQuantity = operation switch
                          {
                              StockOperation.Add => Quantity.Value + quantity,
                              StockOperation.Remove => Quantity.Value - quantity,
                              _ => throw new InvalidOperationException($"Invalid operation: {operation}")
                          };

        if (newQuantity < 0)
            throw new InsufficientStockException(Name, quantity, Quantity.Value);

        Quantity = new StockLevel(newQuantity);
    }
}