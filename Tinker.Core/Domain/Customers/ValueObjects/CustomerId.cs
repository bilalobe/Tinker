namespace Tinker.Core.Domain.Customers.ValueObjects;

public record CustomerId(Guid Value)
{
    public static CustomerId New()
    {
        return new CustomerId(Guid.NewGuid());
    }
}