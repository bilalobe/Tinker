namespace Tinker.Infrastructure.Processing.Tasks.Interfaces;

public interface ILoyaltyTaskHandler
{
    Task ProcessLoyaltyUpdate(Customer customer, decimal totalAmount);
}