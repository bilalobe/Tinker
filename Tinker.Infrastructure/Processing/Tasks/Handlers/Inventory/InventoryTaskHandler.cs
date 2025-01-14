// Services/Implementations/InventoryTaskHandler.cs

using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Processing.Tasks.Interfaces;

namespace Tinker.Infrastructure.Processing.Tasks.Handlers.Inventory;

public class InventoryTaskHandler(
    IApplicationDbContext         context,
    INotificationService          notificationService,
    ILogger<InventoryTaskHandler> logger)
    : IInventoryTaskHandler
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task ProcessInventoryUpdate(Dictionary<int, Product> products, List<OrderItem> orderItems)
    {
        try
        {
            foreach (var item in orderItems)
            {
                var product = products[item.ProductId];
                product.Quantity -= item.Quantity;
                context.Products.Update(product);
                CheckLowStockAlert(product);
            }

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing inventory update");
            throw;
        }
    }

    private void CheckLowStockAlert(Product product)
    {
        if (product.Quantity <= product.MinimumStockLevel)
            _notificationService.SendLowStockAlert(product.Reference, product.Quantity);
    }
}