namespace Tinker.Core.Domain.Common.ValueObjects;

public abstract record ValueObject
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null) return false;
        return left?.Equals(right!) != false;
    }
}