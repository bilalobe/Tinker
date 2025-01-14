namespace Tinker.Core.Domain.Checkout.ValueObjects;

public record Money(decimal Amount)
{
    public static Money operator +(Money a, Money b)
    {
        return new Money(a.Amount + b.Amount);
    }

    public static Money operator -(Money a, Money b)
    {
        return new Money(a.Amount - b.Amount);
    }
}