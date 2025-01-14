using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Customers.Repositories;
using Tinker.Core.Services.Loyalty.Interfaces;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Loyalty;

public class LoyaltyService(ICustomerRepository customerRepository, ILogger<LoyaltyService> logger)
    : ILoyaltyService
{
    public async Task UpdatePoints(int customerId, decimal amount)
    {
        var customer = await customerRepository.GetByIdAsync(customerId)
                       ?? throw new NotFoundException($"Customer {customerId} not found");

        var points = await CalculatePoints(amount);
        customer.UpdateLoyaltyPoints(points);
        customer.MembershipTier = await DetermineMembershipTier(customer.LoyaltyPoints);

        await customerRepository.UpdateAsync(customer);
        logger.LogInformation("Updated loyalty points for customer {CustomerId}: +{Points}", customerId, points);
    }

    public async Task<int> CalculatePoints(decimal amount)
    {
        // Points calculation logic - can be made configurable
        return await Task.FromResult((int)(amount / 10));
    }

    public async Task<string> DetermineMembershipTier(int points)
    {
        return await Task.FromResult(points switch
                                     {
                                         >= 1000 => "Platinum",
                                         >= 500 => "Gold",
                                         >= 200 => "Silver",
                                         _ => "Standard"
                                     });
    }

    public async Task RedeemPoints(int customerId, int points, decimal discount)
    {
        var customer = await customerRepository.GetByIdAsync(customerId)
                       ?? throw new NotFoundException($"Customer {customerId} not found");

        if (customer.LoyaltyPoints < points)
            throw new ValidationException("Insufficient loyalty points");

        customer.LoyaltyPoints -= points;
        await customerRepository.UpdateAsync(customer);

        logger.LogInformation("Redeemed {Points} points for customer {CustomerId}", points, customerId);
    }
}