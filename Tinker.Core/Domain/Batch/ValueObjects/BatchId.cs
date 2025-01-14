namespace Tinker.Core.Domain.Batch.ValueObjects;

public record BatchId(Guid Value)
{
    public static BatchId New()
    {
        return new BatchId(Guid.NewGuid());
    }
}