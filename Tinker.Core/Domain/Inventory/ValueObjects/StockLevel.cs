namespace Tinker.Core.Domain.Inventory.ValueObjects;

public record StockLevel
{
    private const int MinValue = 0;

    public StockLevel(int value)
    {
        if (value < MinValue)
            throw new ArgumentException("Stock level cannot be negative", nameof(value));

        Value = value;
    }

    public int Value { get; }

    public StockLevel Add(int quantity)
    {
        return new StockLevel(Value + quantity);
    }

    public StockLevel Remove(int quantity)
    {
        return new StockLevel(Value - quantity);
    }

    public static implicit operator int(StockLevel level)
    {
        return level.Value;
    }
}