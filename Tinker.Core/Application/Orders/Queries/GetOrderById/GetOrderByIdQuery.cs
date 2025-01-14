namespace Tinker.Core.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery(int orderId)
{
    public int OrderId { get; } = orderId;
}