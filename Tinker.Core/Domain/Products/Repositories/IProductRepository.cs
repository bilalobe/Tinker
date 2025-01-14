using Tinker.Core.Domain.Products.Aggregates;

namespace Tinker.Core.Domain.Products.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product                                   product);
    Task UpdateAsync(Product                                product);
    Task DeleteAsync(int                                    id);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
    Task<IEnumerable<Product>> GetExpiringProductsAsync(int daysThreshold);
}