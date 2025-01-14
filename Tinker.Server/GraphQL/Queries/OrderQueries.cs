using StackExchange.Redis;
using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Server.GraphQL.Queries;

[ExtendObjectType("Query")]
public class OrderQueries
{
    [UseDbContext(typeof(ApplicationDbContext))]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Order> GetOrders([ScopedService] ApplicationDbContext context)
    {
        return context.Orders.Include(o => o.Items);
    }

    public async Task<Order?> GetOrderById(
        [Service] IOrderService orderService,
        int                     id)
    {
        return await orderService.GetOrderById(id);
    }
}