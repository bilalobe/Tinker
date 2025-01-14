using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Security.Compliance.Models;
using NotFoundException = Tinker.Shared.Exceptions.NotFoundException;

namespace Tinker.Infrastructure.Security.Compliance.Services;

public class RxComplianceService(
    IApplicationDbContext        context,
    INotificationService         notificationService,
    ILogger<RxComplianceService> logger)
    : IRxComplianceService
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task ValidateRxTransaction(Order order)
    {
        foreach (var item in order.Items)
        {
            var product = await context.Products.FindAsync(item.ProductId)
                          ?? throw new NotFoundException($"Product {item.ProductId} not found");

            if (product.RequiresRx && !await ValidateRxRequirement(product, order))
            {
                await _notificationService.SendComplianceAlert(new ComplianceAlert
                {
                    Type = "RxRequired",
                    Reference = product.Reference,
                    Details = "Prescription validation failed"
                });
                throw new ComplianceException($"Prescription required for {product.Name}");
            }
        }
    }

    private async Task<bool> ValidateRxRequirement(Product product, Order order)
    {
        if (!product.RequiresRx) return true;

        var prescription = await context.Prescriptions
                               .FirstOrDefaultAsync(p => p.Id == order.PrescriptionId)
                           ?? throw new NotFoundException($"Prescription {order.PrescriptionId} not found");

        return await ValidatePrescription(prescription);
    }

    private async Task<bool> ValidatePrescription(Prescription prescription)
    {
        if (prescription.ExpiryDate < DateTime.UtcNow)
        {
            logger.LogWarning("Prescription {Id} expired on {Date}",
                prescription.Id, prescription.ExpiryDate);
            return false;
        }

        if (prescription.RemainingRefills <= 0)
        {
            logger.LogWarning("Prescription {Id} has no remaining refills",
                prescription.Id);
            return false;
        }

        await UpdatePrescriptionUsage(prescription);
        return true;
    }

    private async Task UpdatePrescriptionUsage(Prescription prescription)
    {
        prescription.RemainingRefills--;
        prescription.LastUsedDate = DateTime.UtcNow;
        context.Prescriptions.Update(prescription);
        await context.SaveChangesAsync();

        logger.LogInformation("Updated prescription {Id}. Remaining refills: {Refills}",
            prescription.Id, prescription.RemainingRefills);
    }
}