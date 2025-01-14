using GreenDonut;
using MediatR;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Application.Sales.Commands.ProcessSale;

public record ProcessSaleCommand : IRequest<Result<OrderDto>>
{
    public required OrderDto Order { get; init; }
}