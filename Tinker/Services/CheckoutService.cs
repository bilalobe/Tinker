public class CheckoutService
{
    public void RedeemPoints(Customer customer, Order order, int pointsToRedeem)
    {
        if (customer.LoyaltyPoints >= pointsToRedeem)
        {
            decimal discount = CalculateDiscount(pointsToRedeem);
            order.TotalAmount -= discount;
            customer.LoyaltyPoints -= pointsToRedeem;
        }
        else
        {
            // Handle insufficient points scenario
        }
    }

    private decimal CalculateDiscount(int points)
    {
        // Example: Each point is worth $0.05
        return points * 0.05m;
    }
}
