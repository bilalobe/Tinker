using Tinker.Shared.DTOs.Extras;

namespace Tinker.Infrastructure.Core.Data.Interfaces;

public interface INotifier
{
    Task SendAlert(AlertType                                            type, AlertData        data);
    Task SendNotification(NotificationType                              type, NotificationData data);
    Task<IEnumerable<NotificationLog>> GetNotificationHistory(DateRange range);
}