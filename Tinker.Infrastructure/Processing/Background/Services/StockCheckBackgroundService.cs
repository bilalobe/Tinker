using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Processing.Configuration;

namespace Tinker.Infrastructure.Processing.Background.Services;

public class StockCheckBackgroundService : BackgroundServiceBase
{
    private readonly BackgroundServiceSettings _settings;

    public StockCheckBackgroundService(
        ILogger<StockCheckBackgroundService> logger,
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

        var lowStockProducts = await context.Products
            .Where(p => p.Quantity <= p.MinimumStockLevel)
            .ToListAsync(stoppingToken);

        foreach (var product in lowStockProducts)
        {
            Logger.LogWarning(
                "Low stock alert for product {ProductName} (ID: {ProductId}). Current quantity: {Quantity}",
                product.Name,
                product.Id,
                product.Quantity);

            await notificationService.SendLowStockAlert(product);
        }

        Logger.LogInformation(
            "Stock check completed. Found {Count} products with low stock",
            lowStockProducts.Count);
    }
}
