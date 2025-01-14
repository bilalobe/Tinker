using GreenDonut;
using MediatR;

namespace Tinker.Core.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<Result<>>
{
    public required string Name { get; init; }
    public required string ContactDetails { get; init; }
}