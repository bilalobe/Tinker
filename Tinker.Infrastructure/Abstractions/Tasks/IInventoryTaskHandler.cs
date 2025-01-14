namespace Tinker.Infrastructure.Processing.Tasks.Interfaces;

public interface IInventoryTaskHandler
{
    Task ProcessInventoryUpdate(Dictionary<int, Product> products, List<OrderItem> orderItems);
}