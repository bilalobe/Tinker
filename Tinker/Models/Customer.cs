using System.ComponentModel.DataAnnotations;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = default!;

    [EmailAddress]
    public string Email { get; set; } = default!;

    [Phone]
    public string PhoneNumber { get; set; } = default!;

    public ICollection<Order> PurchaseHistory { get; set; } = new List<Order>();

    public int LoyaltyPoints { get; set; }

    public string MembershipTier { get; set; } = "Standard";

    public DateTime LastPurchaseDate { get; set; }
}
