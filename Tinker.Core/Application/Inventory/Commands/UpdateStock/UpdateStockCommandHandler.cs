using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Inventory.ValueObjects;

namespace Tinker.Core.Application.Inventory.Commands.UpdateStock;

public class UpdateStockCommandHandler(IApplicationDbContext context, ILogger<UpdateStockCommandHandler> logger)
    : IRequestHandler<UpdateStockCommand, Result<>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        var product = await _context.Products.FindAsync(request.ProductId, ct);
        if (product == null)
            return Result.Failure($"Product {request.ProductId} not found");

        try
        {
            product.UpdateStock(request.Quantity, Enum.Parse<StockOperation>(request.Operation, true));
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating stock for product {ProductId}", request.ProductId);
            return Result.Failure(ex.Message);
        }
    }
}