using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Core.Data.Interfaces;

namespace Tinker.Infrastructure.Integration.Notifications.Persistence;

public class NotificationStore(IApplicationDbContext context, ILogger<NotificationStore> logger)
    : INotificationStore
{
    private readonly ILogger<NotificationStore> _logger = logger;

    public async Task LogNotification(NotificationLog notification)
    {
        await context.Notifications.AddAsync(notification);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<NotificationLog>> GetNotificationHistory(DateTime start, DateTime end)
    {
        return await context.Notifications
            .Where(n => n.Timestamp >= start && n.Timestamp <= end)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }
}