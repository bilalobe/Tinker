namespace Tinker.Core.Domain.Orders.ValueObjects;

public record OrderId(Guid Value)
{
    public static OrderId New()
    {
        return new OrderId(Guid.NewGuid());
    }
}