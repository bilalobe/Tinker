namespace Tinker.Shared.DTOs.Customers;

public record CustomerDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public int LoyaltyPoints { get; init; }
    public required string MembershipTier { get; init; }
}