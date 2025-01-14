namespace Tinker.Client.Infrastructure.State.Models;

public record CartStateModel
{
    public List<CartItem> Items { get; init; } = new();
    public decimal Total { get; init; }
    public int ItemCount { get; init; }
    public bool IsCheckingOut { get; init; }
}