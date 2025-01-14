namespace Tinker.Core.Domain.Batch.Repositories;

public interface IBatchRepository
{
    Task<Batch?> GetByIdAsync(string batchNumber);
    Task<IEnumerable<Batch>> GetAllAsync();
    Task AddAsync(Batch                                       batch);
    Task UpdateAsync(Batch                                    batch);
    Task DeleteAsync(string                                   batchNumber);
    Task<IEnumerable<Batch>> GetExpiringBatchesAsync(DateTime thresholdDate);
}