using Tinker.Shared.Exceptions;

namespace Tinker.Server.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class ProductMutations
{
    [Error(typeof(ValidationException))]
    public async Task<ProductPayload> CreateProduct(
        [Service] IProductService productService,
        CreateProductInput        input)
    {
        await productService.AddProduct(input.Product);
        return new ProductPayload(input.Product);
    }

    [Error(typeof(ValidationException))]
    [Error(typeof(NotFoundException))]
    public async Task<ProductPayload> UpdateProduct(
        [Service] IProductService productService,
        UpdateProductInput        input)
    {
        await productService.UpdateProduct(input.Product);
        return new ProductPayload(input.Product);
    }

    [Error(typeof(ValidationException))]
    [Error(typeof(NotFoundException))]
    public async Task<StockUpdatePayload> UpdateStock(
        [Service] IProductService productService,
        StockUpdateInput          input)
    {
        await productService.UpdateStock(
            input.ProductId,
            input.Quantity,
            input.Operation);

        return new StockUpdatePayload(true);
    }

    [Error(typeof(ValidationException))]
    public async Task<BatchUpdatePayload> UpdateBatch(
        [Service] IProductService productService,
        BatchUpdateInput          input)
    {
        await productService.UpdateBatch(
            input.ProductId,
            input.BatchNumber,
            input.ExpiryDate);
        return new BatchUpdatePayload(true);
    }

    [Error(typeof(ValidationException))]
    public async Task<RxStatusPayload> UpdateRxStatus(
        [Service] IProductService productService,
        RxStatusInput             input)
    {
        await productService.UpdateRxStatus(
            input.ProductId,
            input.RequiresRx);
        return new RxStatusPayload(true);
    }
}