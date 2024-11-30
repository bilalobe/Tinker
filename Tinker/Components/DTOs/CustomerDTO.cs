public class CustomerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public int LoyaltyPoints { get; set; }
    public string MembershipTier { get; set; } = default!;
}
