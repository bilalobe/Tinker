using Tinker.Core.Domain.Common.Interfaces;

namespace Tinker.Core.Domain.Inventory.Events;

public record LowStockEvent(ProductId ProductId, string Reference, int CurrentQuantity, int MinimumStockLevel)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}