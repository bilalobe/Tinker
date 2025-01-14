using Tinker.Core.Domain.Inventory.Aggregates;
using Tinker.Shared.DTOs.Inventory;

namespace Tinker.Core.Services.Products.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProducts();
    Task<ProductDto?> GetProductById(int                    id);
    Task AddProduct(ProductDto                              productDto);
    Task UpdateProduct(ProductDto                           productDto);
    Task<Product?> GetProductEntityById(int                 id);
    Task UpdateProductEntity(Product                        product);
    Task UpdateStock(int                                    productId, int quantity, string operation);
    Task<IEnumerable<ProductDto>> GetLowStockProducts(int   threshold = 10);
    Task<IEnumerable<ProductDto>> GetExpiringProducts(int   daysThreshold);
    Task<IEnumerable<ProductDto>> GetProductsByBatch(string batchNumber);
    Task<IEnumerable<ProductDto>> GetRxProducts();
    Task UpdateBatch(int    productId, string batchNumber, DateTime expiryDate);
    Task UpdateRxStatus(int productId, bool   requiresRx);
}