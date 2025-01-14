using Azure.Core.Pipeline;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Processing.Tasks.Interfaces;
using Order = StackExchange.Redis.Order;

namespace Tinker.Infrastructure.Processing.Background.Services;

public class TaskService(
    ILoyaltyTaskHandler      loyaltyTaskHandler,
    IInventoryTaskHandler    inventoryTaskHandler,
    INotificationTaskHandler notificationTaskHandler,
    ILogger<TaskService>     logger)
    : ITaskService
{
    private readonly ILogger<TaskService> _logger = logger;

    public async Task RunScheduledTasks(CancellationToken stoppingToken)
    {
        var retryPolicy = RetryPolicy.GetRetryPolicy();

        await retryPolicy.ExecuteAsync(async () =>
        {
            // Example task execution
            await loyaltyTaskHandler.ProcessLoyaltyUpdate(new Customer(), 100);
            await inventoryTaskHandler.ProcessInventoryUpdate(new Dictionary<int, Product>(), new List<OrderItem>());
            await notificationTaskHandler.SendOrderNotifications(new Order());
        });
    }
}