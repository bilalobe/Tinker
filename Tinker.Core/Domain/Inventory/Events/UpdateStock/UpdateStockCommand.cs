using GreenDonut;
using MediatR;
using Tinker.Core.Domain.Inventory.ValueObjects;

namespace Tinker.Core.Domain.Inventory.Events.UpdateStock;

public record UpdateStockCommand : IRequest<Result<>>
{
    public required ProductId ProductId { get; init; }
    public required int Quantity { get; init; }
    public required StockOperation Operation { get; init; }
}