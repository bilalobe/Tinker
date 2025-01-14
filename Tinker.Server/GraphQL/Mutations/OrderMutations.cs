using Tinker.Shared.DTOs.Orders;

namespace Tinker.Server.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class OrderMutations
{
    public async Task<OrderPayload> CreateOrder(
        [Service] IOrderService orderService,
        OrderInput              input)
    {
        var order = await orderService.CreateOrder(input);
        return new OrderPayload(order, true);
    }

    public async Task<OrderPayload> UpdateOrderStatus(
        [Service] IOrderService orderService,
        int                     orderId,
        OrderStatus             status)
    {
        var order = await orderService.UpdateStatus(orderId, status);
        return new OrderPayload(order, true);
    }
}