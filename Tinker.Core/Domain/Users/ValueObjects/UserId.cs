using Tinker.Core.Domain.Common.ValueObjects;

namespace Tinker.Core.Domain.Users.ValueObjects;

public record UserId(Guid Value) : ValueObject
{
    public static UserId New()
    {
        return new UserId(Guid.NewGuid());
    }
}