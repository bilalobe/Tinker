using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Security.Compliance.Models;

namespace Tinker.Infrastructure.Integration.Messaging.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly NotificationSettings _settings;
    private readonly IEmailService _emailService;
    private readonly INotificationStore _notificationStore;

    public NotificationService(
        ILogger<NotificationService> logger,
        IOptions<NotificationSettings> settings,
        IEmailService emailService,
        INotificationStore notificationStore)
    {
        _logger = logger;
        _settings = settings.Value;
        _emailService = emailService;
        _notificationStore = notificationStore;
    }

    public async Task SendOrderConfirmation(int orderId)
    {
        try
        {
            var notification = new NotificationData
            {
                Message = $"Order {orderId} has been confirmed",
                Recipient = "customer@example.com" // Should come from order details
            };

            await _notificationStore.LogNotification(new NotificationLog
            {
                Type = NotificationType.Email,
                Message = notification.Message,
                Recipient = notification.Recipient,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Order confirmation sent for Order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation for {OrderId}", orderId);
            throw;
        }
    }

    public async Task SendLowStockAlert(string productReference, int quantity)
    {
        if (!_settings.EnableStockAlerts) return;

        try
        {
            var alert = new AlertData
            {
                Message = $"Low stock alert for product {productReference}",
                Details = $"Current quantity: {quantity}"
            };

            if (quantity <= _settings.LowStockThreshold)
            {
                await _emailService.SendAlertEmail(alert);

                _logger.LogWarning(
                    "Low stock alert for product {Reference}: {Quantity} remaining",
                    productReference, quantity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send low stock alert for {Reference}", productReference);
            throw;
        }
    }

    public async Task SendExpiryAlert(
        string reference,
        string batchNumber,
        DateTime expiryDate,
        InventoryService.ExpiryStatus status)
    {
        try
        {
            var daysUntilExpiry = (expiryDate - DateTime.UtcNow).Days;

            if (daysUntilExpiry <= _settings.ExpiryWarningDays)
            {
                var alert = new AlertData
                {
                    Message = $"Product expiry alert for {reference}",
                    Details = $"Batch {batchNumber} expires on {expiryDate:d}. Status: {status}"
                };

                await _emailService.SendAlertEmail(alert);

                _logger.LogWarning(
                    "Product {Reference} (Batch: {Batch}) will expire on {Date}. Status: {Status}",
                    reference, batchNumber, expiryDate, status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send expiry alert for {Reference}", reference);
            throw;
        }
    }

    public async Task SendRestockAlert(RestockAlert alert)
    {
        try
        {
            var notification = new AlertData
            {
                Message = "Restock Alert",
                Details = $"Product {alert.ProductReference} needs restocking. " +
                          $"Current: {alert.CurrentStock}, Minimum: {alert.MinimumStockLevel}"
            };

            await _emailService.SendAlertEmail(notification);

            _logger.LogWarning(
                "Restock needed for product {Reference}. Current stock: {Current}, Minimum: {Minimum}",
                alert.ProductReference, alert.CurrentStock, alert.MinimumStockLevel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send restock alert for {Reference}",
                alert.ProductReference);
            throw;
        }
    }

    public async Task SendComplianceAlert(ComplianceAlert alert)
    {
        try
        {
            var notification = new AlertData
            {
                Message = $"Compliance Alert: {alert.Type}",
                Details = alert.Details
            };

            await _emailService.SendAlertEmail(notification);

            _logger.LogWarning(
                "Compliance alert: {Type} for {Reference}. Details: {Details}",
                alert.Type, alert.Reference, alert.Details);

            await _notificationStore.LogNotification(new NotificationLog
            {
                Type = NotificationType.Email,
                Message = notification.Message,
                Recipient = "compliance@pharmacy.com",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send compliance alert for {Reference}",
                alert.Reference);
            throw;
        }
    }
}