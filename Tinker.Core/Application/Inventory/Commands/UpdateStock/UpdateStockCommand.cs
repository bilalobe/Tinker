using GreenDonut;
using MediatR;

namespace Tinker.Core.Application.Inventory.Commands.UpdateStock;

public record UpdateStockCommand : IRequest<Result<>>
{
    public required int ProductId { get; init; }
    public required int Quantity { get; init; }
    public required string Operation { get; init; }
}