using Tinker.Core.Domain.Batch.Events;
using Tinker.Core.Domain.Batch.ValueObjects;
using Tinker.Core.Domain.Common.Models;

namespace Tinker.Core.Domain.Batch.Entities;

public class Batch : AggregateRoot
{
    private Batch(BatchId id, string batchNumber, DateTime expiryDate)
    {
        Id = id;
        BatchNumber = batchNumber;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        Status = BatchStatus.Active;
    }

    public BatchId Id { get; private set; }
    public string BatchNumber { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public BatchStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public static Batch Create(string batchNumber, DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(batchNumber))
            throw new ArgumentException("Batch number cannot be empty", nameof(batchNumber));

        if (expiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(expiryDate));

        var batch = new Batch(BatchId.New(), batchNumber, expiryDate);
        batch.AddDomainEvent(new BatchCreatedEvent(batch));
        return batch;
    }

    public void UpdateExpiryDate(DateTime newExpiryDate)
    {
        if (newExpiryDate <= DateTime.UtcNow)
            throw new ArgumentException("New expiry date must be in the future", nameof(newExpiryDate));

        ExpiryDate = newExpiryDate;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    public void Deactivate()
    {
        Status = BatchStatus.Inactive;
    }
}