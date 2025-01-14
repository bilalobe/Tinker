using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Services.Sales.Interfaces;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Application.Sales.Commands.ProcessSale;

public class ProcessSaleCommandHandler(ISalesService salesService, ILogger<ProcessSaleCommandHandler> logger)
    : IRequestHandler<ProcessSaleCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(ProcessSaleCommand request, CancellationToken ct)
    {
        try
        {
            var order = await salesService.ProcessSale(request.Order);
            return Result.Success(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing sale");
            return Result.Failure<OrderDto>(ex.Message);
        }
    }
}