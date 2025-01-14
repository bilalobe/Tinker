namespace Tinker.Server.GraphQL.Queries;

[ExtendObjectType("Query")]
public class BatchQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<BatchResponse>> GetBatches(
        [Service] IProductService productService,
        string?                   batchNumber = null)
    {
        var products = await productService.GetProductsByBatch(batchNumber ?? string.Empty);
        return products.Select(p => new BatchResponse(p.BatchNumber, p.ExpiryDate, p.Quantity));
    }

    public async Task<BatchResponse?> GetBatch(
        [Service] IProductService productService,
        string                    batchNumber)
    {
        var product = (await productService.GetProductsByBatch(batchNumber)).FirstOrDefault();
        return product == null ? null : new BatchResponse(product.BatchNumber, product.ExpiryDate, product.Quantity);
    }
}