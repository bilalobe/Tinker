using GreenDonut;
using MediatR;
using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Core.Domain.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<Result<>>
{
    public required ProductDto Product { get; init; }
}