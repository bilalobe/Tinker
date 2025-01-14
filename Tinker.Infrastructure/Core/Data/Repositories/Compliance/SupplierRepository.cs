using Microsoft.EntityFrameworkCore;
using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Infrastructure.Core.Data.Repositories.Compliance;

public class SupplierRepository(ApplicationDbContext context) : ISupplierRepository
{
    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await context.Suppliers.FindAsync(id);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await context.Suppliers.ToListAsync();
    }

    public async Task AddAsync(Supplier supplier)
    {
        await context.Suppliers.AddAsync(supplier);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        context.Suppliers.Update(supplier);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await GetByIdAsync(id);
        if (supplier != null)
        {
            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync();
        }
    }
}