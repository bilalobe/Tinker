namespace Tinker.Core.Services.Batch.Interfaces;

public interface IBatchService
{
    Task<Batch> CreateBatch(string                  batchNumber, DateTime expiryDate);
    Task<Batch> GetBatchByNumber(string             batchNumber);
    Task UpdateExpiryDate(string                    batchNumber, DateTime newExpiryDate);
    Task<IEnumerable<Batch>> GetExpiringBatches(int daysThreshold);
}