using Tinker.Shared.Exceptions;

namespace Tinker.Server.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class BatchMutations
{
    [Error(typeof(ValidationException))]
    [Error(typeof(NotFoundException))]
    public async Task<BatchPayload> UpdateBatch(
        [Service] IProductService productService,
        BatchInput                input)
    {
        await productService.UpdateBatch(
            input.ProductId,
            input.BatchNumber,
            input.ExpiryDate);

        var product = await productService.GetProductById(input.ProductId);
        var response = new BatchResponse(
            product.BatchNumber,
            product.ExpiryDate,
            product.Quantity);

        return new BatchPayload(response, true);
    }
}