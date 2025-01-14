using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Services.Orders.Interfaces;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Application.Orders.Commands.CreateCommand;

public class CreateOrderCommandHandler(IOrderService orderService, ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        try
        {
            var order = await orderService.CreateOrder(request.Order);
            return Result.Success(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            return Result.Failure<OrderDto>(ex.Message);
        }
    }
}