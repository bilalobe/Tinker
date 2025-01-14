using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Infrastructure.Core.Data.Repositories.Products;

public class BatchRepository(ApplicationDbContext context) : IBatchRepository
{
    public async Task<Batch?> GetByIdAsync(string batchNumber)
    {
        return await context.Batches.FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);
    }

    public async Task<IEnumerable<Batch>> GetAllAsync()
    {
        return await context.Batches.ToListAsync();
    }

    public async Task AddAsync(Batch batch)
    {
        await context.Batches.AddAsync(batch);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Batch batch)
    {
        context.Batches.Update(batch);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string batchNumber)
    {
        var batch = await GetByIdAsync(batchNumber);
        if (batch != null)
        {
            context.Batches.Remove(batch);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Batch>> GetExpiringBatchesAsync(DateTime thresholdDate)
    {
        return await context.Batches
            .Where(b => b.ExpiryDate <= thresholdDate)
            .ToListAsync();
    }
}