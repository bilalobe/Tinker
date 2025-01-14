using GreenDonut;
using MediatR;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Application.Orders.Commands.CreateCommand;

public record CreateOrderCommand : IRequest<Result<OrderDto>>
{
    public required OrderDto Order { get; init; }
}