using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Processing.Tasks.Interfaces;

namespace Tinker.Infrastructure.Processing.Tasks.Handlers.Notifications;

public class NotificationTaskHandler(INotificationService notificationService, ILogger<NotificationTaskHandler> logger)
    : INotificationTaskHandler
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task SendOrderNotifications(Order order)
    {
        try
        {
            await SendOrderConfirmation(order);
            await SendInventoryAlerts(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending order notifications for order {OrderId}", order.Id);
            throw;
        }
    }

    private async Task SendOrderConfirmation(Order order)
    {
        await _notificationService.SendOrderConfirmation(order.Id);
    }

    private async Task SendInventoryAlerts(Order order)
    {
        logger.LogInformation("Inventory alerts processed for Order {OrderId}", order.Id);
        await Task.CompletedTask;
    }
}