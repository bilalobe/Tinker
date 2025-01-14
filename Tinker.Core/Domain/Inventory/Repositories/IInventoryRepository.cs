using Tinker.Core.Domain.Inventory.Aggregates;

namespace Tinker.Core.Domain.Inventory.Repositories;

public interface IInventoryRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product                                   product);
    Task UpdateAsync(Product                                product);
    Task DeleteAsync(int                                    id);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
    Task<IEnumerable<Product>> GetExpiringProductsAsync(int daysThreshold);
}