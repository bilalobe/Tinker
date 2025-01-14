using Tinker.Core.Domain.Common.Models;
using Tinker.Core.Domain.Customers.Events;
using Tinker.Core.Domain.Customers.ValueObjects;

namespace Tinker.Core.Domain.Customers.Entities;

public class Customer(CustomerId id, string name, string email, string phoneNumber)
    : AggregateRoot
{
    public CustomerId Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string PhoneNumber { get; private set; } = phoneNumber;
    public int LoyaltyPoints { get; private set; }
    public string MembershipTier { get; private set; } = "Standard";
    public DateTime LastPurchaseDate { get; private set; } = DateTime.UtcNow;

    public void UpdateLoyaltyPoints(int points)
    {
        LoyaltyPoints += points;
        UpdateMembershipTier();
        AddDomainEvent(new LoyaltyPointsUpdatedEvent(Id, points));
    }

    private void UpdateMembershipTier()
    {
        MembershipTier = LoyaltyPoints switch
                         {
                             >= 1000 => "Platinum",
                             >= 500 => "Gold",
                             >= 200 => "Silver",
                             _ => "Standard"
                         };
    }
}