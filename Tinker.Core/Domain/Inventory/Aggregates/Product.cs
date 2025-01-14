// Tinker.Core/Domain/Inventory/Aggregates/Product.cs

using Tinker.Core.Domain.Checkout.ValueObjects;
using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Inventory.Events;
using Tinker.Core.Domain.Inventory.ValueObjects;

namespace Tinker.Core.Domain.Inventory.Aggregates;

public class Product : AggregateRoot
{
    private readonly List<BatchItem> _batches = new();

    public ProductId Id { get; private set; }
    public string Reference { get; private set; }
    public string Name { get; private set; }
    public Money Price { get; private set; }
    public StockLevel Quantity { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public IReadOnlyCollection<BatchItem> Batches => _batches.AsReadOnly();

    public void UpdateStock(int quantity, StockOperation operation)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        var newStockLevel = operation switch
                            {
                                StockOperation.Add => Quantity.Add(quantity),
                                StockOperation.Remove => Quantity.Remove(quantity),
                                _ => throw new InvalidOperationException($"Unsupported stock operation: {operation}")
                            };

        if (newStockLevel.Value < 0)
            throw new InsufficientStockException(Reference, quantity, Quantity.Value);

        Quantity = newStockLevel;

        if (ShouldNotifyLowStock()) AddDomainEvent(new LowStockEvent(Id, Reference, Quantity.Value));
    }

    private bool ShouldNotifyLowStock()
    {
        return Quantity.Value <= MinimumStockLevel;
    }
}