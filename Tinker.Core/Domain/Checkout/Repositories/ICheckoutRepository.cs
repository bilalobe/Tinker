using Tinker.Core.Domain.Checkout.Entities;

namespace Tinker.Core.Domain.Checkout.Repositories;

public interface ICheckoutRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task AddAsync(Order    order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int   id);
}