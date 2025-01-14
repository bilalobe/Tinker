using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Services.Inventory.Interfaces;

namespace Tinker.Core.Domain.Inventory.Events.UpdateStock;

public class UpdateStockCommandHandler(IInventoryService inventoryService, ILogger<UpdateStockCommandHandler> logger)
    : IRequestHandler<UpdateStockCommand, Result<>>
{
    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        try
        {
            await inventoryService.UpdateStock(request.ProductId, request.Quantity, request.Operation);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating stock for product {ProductId}", request.ProductId);
            return Result.Failure(ex.Message);
        }
    }
}