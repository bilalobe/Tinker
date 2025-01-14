namespace Tinker.Shared.Exceptions;

public class NotFoundException : BusinessException
{
    public NotFoundException(Type entityType, object id)
        : base(
            $"Entity of type {entityType.Name} with id {id} was not found",
            "ENTITY_NOT_FOUND",
            new Dictionary<string, object>
            {
                ["entityType"] = entityType.Name,
                ["entityId"] = id
            })
    {
        EntityType = entityType;
        EntityId = id;
    }

    public NotFoundException(string message, Type entityType, object id)
        : base(
            message,
            "ENTITY_NOT_FOUND",
            new Dictionary<string, object>
            {
                ["entityType"] = entityType.Name,
                ["entityId"] = id
            })
    {
        EntityType = entityType;
        EntityId = id;
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public Type EntityType { get; }
    public object EntityId { get; }
}