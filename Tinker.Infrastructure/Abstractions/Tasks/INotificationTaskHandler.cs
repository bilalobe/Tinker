using StackExchange.Redis;

namespace Tinker.Infrastructure.Processing.Tasks.Interfaces;

public interface INotificationTaskHandler
{
    Task SendOrderNotifications(Order order);
}