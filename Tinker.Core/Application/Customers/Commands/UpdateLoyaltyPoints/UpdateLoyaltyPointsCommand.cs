using GreenDonut;
using MediatR;
using Tinker.Core.Domain.Customers.ValueObjects;

namespace Tinker.Core.Application.Customers.Commands.UpdateLoyaltyPoints;

public record UpdateLoyaltyPointsCommand : IRequest<Result<>>
{
    public required CustomerId CustomerId { get; init; }
    public required int Points { get; init; }
}