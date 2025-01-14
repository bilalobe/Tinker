using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Server.GraphQL.Queries;

[ExtendObjectType("Query")]
public class ProductQueries
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<ProductDto>> GetProducts(
        [Service] IProductService productService)
    {
        return await productService.GetProducts();
    }

    public async Task<ProductDto?> GetProduct(
        [Service] IProductService productService,
        [ID]      int             id)
    {
        return await productService.GetProductById(id);
    }

    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<ProductDto>> GetLowStockProducts(
        [Service] IProductService productService,
        int                       threshold = 10)
    {
        return await productService.GetLowStockProducts(threshold);
    }
}