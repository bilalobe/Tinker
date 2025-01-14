using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Infrastructure.Core.Data.Repositories.Orders;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await context.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await context.Orders.ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var order = await GetByIdAsync(id);
        if (order != null)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }
    }
}