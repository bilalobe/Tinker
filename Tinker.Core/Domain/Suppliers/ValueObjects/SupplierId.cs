namespace Tinker.Core.Domain.Suppliers.ValueObjects;

public record SupplierId(Guid Value)
{
    public static SupplierId New()
    {
        return new SupplierId(Guid.NewGuid());
    }
}