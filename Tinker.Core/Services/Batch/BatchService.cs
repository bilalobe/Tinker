using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Batch.Repositories;
using Tinker.Core.Services.Batch.Interfaces;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Batch;

public class BatchService(IBatchRepository batchRepository, ILogger<BatchService> logger)
    : IBatchService
{
    public async Task<Batch> CreateBatch(string batchNumber, DateTime expiryDate)
    {
        var batch = Domain.Batch.Entities.Batch.Create(batchNumber, expiryDate);
        await batchRepository.AddAsync(batch);
        logger.LogInformation("Batch {BatchNumber} created successfully", batchNumber);
        return batch;
    }

    public async Task<Batch> GetBatchByNumber(string batchNumber)
    {
        var batch = await batchRepository.GetByIdAsync(batchNumber);
        return batch ?? throw new NotFoundException($"Batch {batchNumber} not found");
    }

    public async Task UpdateExpiryDate(string batchNumber, DateTime newExpiryDate)
    {
        var batch = await GetBatchByNumber(batchNumber);
        batch.UpdateExpiryDate(newExpiryDate);
        await batchRepository.UpdateAsync(batch);
        logger.LogInformation("Updated expiry date for batch {BatchNumber}", batchNumber);
    }

    public async Task<IEnumerable<Batch>> GetExpiringBatches(int daysThreshold)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await batchRepository.GetExpiringBatchesAsync(thresholdDate);
    }
}