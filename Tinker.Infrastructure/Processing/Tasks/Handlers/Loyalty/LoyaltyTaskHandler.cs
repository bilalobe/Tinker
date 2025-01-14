// Services/Implementations/LoyaltyTaskHandler.cs

using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Processing.Tasks.Interfaces;

namespace Tinker.Infrastructure.Processing.Tasks.Handlers.Loyalty;

public class LoyaltyTaskHandler(IApplicationDbContext context, ILogger<LoyaltyTaskHandler> logger)
    : ILoyaltyTaskHandler
{
    public async Task ProcessLoyaltyUpdate(Customer customer, decimal totalAmount)
    {
        try
        {
            var points = CalculateLoyaltyPoints(totalAmount);
            customer.LoyaltyPoints += points;
            UpdateMembershipTier(customer);
            context.Customers.Update(customer);
            await context.SaveChangesAsync();
            logger.LogInformation("Loyalty points updated for customer {CustomerId}: +{Points}", customer.Id, points);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing loyalty update for customer {CustomerId}", customer.Id);
            throw;
        }
    }

    private static int CalculateLoyaltyPoints(decimal amount)
    {
        return (int)(amount / 10);
    }

    private static void UpdateMembershipTier(Customer customer)
    {
        customer.MembershipTier = customer.LoyaltyPoints switch
                                  {
                                      >= 1000 => "Platinum",
                                      >= 500 => "Gold",
                                      >= 200 => "Silver",
                                      _ => "Standard"
                                  };
    }
}