using Tinker.Infrastructure.Security.Compliance.Models;

namespace Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;

public interface IMessageService
{
    Task SendOrderConfirmation(int orderId);
    Task SendLowStockAlert(string productReference, int quantity);
    Task SendExpiryAlert(string reference, string batchNumber, DateTime expiryDate, InventoryService.ExpiryStatus status);
    Task SendRestockAlert(RestockAlert alert);
    Task SendComplianceAlert(ComplianceAlert alert);
}