namespace Tinker.Shared.Exceptions;

public class ConcurrencyException(
    string  entityName,
    object  entityId,
    string? expectedVersion = null,
    string? actualVersion   = null)
    : BusinessException($"Concurrency conflict detected for {entityName} with id {entityId}",
        "CONCURRENCY_ERROR",
        new Dictionary<string, object>
        {
            ["entityName"] = entityName,
            ["entityId"] = entityId,
            ["expectedVersion"] = expectedVersion ?? "unknown",
            ["actualVersion"] = actualVersion ?? "unknown"
        })
{
    public string EntityName { get; } = entityName;
    public object EntityId { get; } = entityId;
    public string? ExpectedVersion { get; } = expectedVersion;
    public string? ActualVersion { get; } = actualVersion;
}