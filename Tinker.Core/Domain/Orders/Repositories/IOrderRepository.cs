using Tinker.Core.Domain.Checkout.Entities;

namespace Tinker.Core.Domain.Orders.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task AddAsync(Order    order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int   id);
}