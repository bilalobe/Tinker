using Microsoft.EntityFrameworkCore;
using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Infrastructure.Core.Data.Repositories.Products;

public class InventoryRepository(ApplicationDbContext context) : IInventoryRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await context.Products
            .Where(p => p.Quantity <= threshold)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetExpiringProductsAsync(int daysThreshold)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await context.Products
            .Where(p => p.ExpiryDate <= thresholdDate)
            .ToListAsync();
    }
}