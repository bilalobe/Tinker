namespace Tinker.Shared.DTOs.Customers;

public record CustomerStatistics
{
    public int TotalOrders { get; init; }
    public decimal TotalSpent { get; init; }
    public int LoyaltyPoints { get; init; }
    public string MembershipTier { get; init; } = string.Empty;
    public DateTime LastPurchaseDate { get; init; }
}