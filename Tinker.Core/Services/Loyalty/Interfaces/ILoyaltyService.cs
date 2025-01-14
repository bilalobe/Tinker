namespace Tinker.Core.Services.Loyalty.Interfaces;

public interface ILoyaltyService
{
    Task UpdatePoints(int                    customerId, decimal amount);
    Task<int> CalculatePoints(decimal        amount);
    Task<string> DetermineMembershipTier(int points);
    Task RedeemPoints(int                    customerId, int points, decimal discount);
}