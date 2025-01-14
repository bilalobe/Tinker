using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Services.Products.Interfaces;

namespace Tinker.Core.Domain.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductService productService, ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, Result<>>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        try
        {
            await productService.UpdateProduct(request.Product);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product");
            return Result.Failure(ex.Message);
        }
    }
}