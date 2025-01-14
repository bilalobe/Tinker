using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Processing.Configuration;

namespace Tinker.Infrastructure.Processing.Background.Services;

public class ExpiryCheckBackgroundService : BackgroundServiceBase
{
    private readonly BackgroundServiceSettings _settings;

    public ExpiryCheckBackgroundService(
        ILogger<ExpiryCheckBackgroundService> logger,
        IServiceProvider serviceProvider,
        IOptions<BackgroundServiceSettings> settings)
        : base(logger, serviceProvider, settings)
    {
        _settings = settings.Value;
    }

    protected override async Task ProcessAsync(CancellationToken stoppingToken)
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var warningDate = DateTime.UtcNow.AddDays(_settings.ExpiryWarningDays);
        var expiringProducts = await context.Products
            .Where(p => p.ExpiryDate <= warningDate && p.Quantity > 0)
            .ToListAsync(stoppingToken);

        foreach (var product in expiringProducts)
        {
            var daysUntilExpiry = (product.ExpiryDate - DateTime.UtcNow).Days;
            Logger.LogWarning(
                "Expiry alert for product {ProductName} (ID: {ProductId}). Expires in {Days} days",
                product.Name,
                product.Id,
                daysUntilExpiry);

            await notificationService.SendExpiryAlert(product, daysUntilExpiry);
        }

        Logger.LogInformation(
            "Expiry check completed. Found {Count} products near expiry",
            expiringProducts.Count);
    }
}
