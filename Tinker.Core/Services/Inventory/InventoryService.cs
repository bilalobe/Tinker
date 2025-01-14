using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Inventory.Events;
using Tinker.Core.Domain.Inventory.ValueObjects;
using Tinker.Core.Domain.Products.Repositories;
using Tinker.Core.Services.Inventory.Interfaces;
using Tinker.Shared.Enums;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Inventory;

public class InventoryService(
    IProductRepository          productRepository,
    INotificationService        notificationService,
    ILogger<InventoryService>   logger,
    IState<InventoryStateModel> state,
    IInventoryHttpClient        client)
    : IInventoryService
{
    private readonly IInventoryHttpClient _client = client;
    private readonly IState<InventoryStateModel> _state = state;

    public async Task<bool> CheckStock(int productId, int quantity)
    {
        var product = await productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            logger.LogWarning("Product {ProductId} not found during stock check", productId);
            return false;
        }

        var validBatchQuantity = product.BatchItems
            .Where(b => b.ExpiryDate > DateTime.UtcNow)
            .Sum(b => b.Quantity);

        var hasStock = validBatchQuantity >= quantity;
        if (!hasStock)
            logger.LogWarning(
                "Insufficient valid stock for product {Reference}. Requested: {Quantity}, Available: {Available}",
                product.Reference, quantity, validBatchQuantity);

        return hasStock;
    }

    public async Task GenerateStockAlert(Product product)
    {
        if (product.Quantity <= product.MinimumStockLevel)
        {
            await notificationService.SendLowStockAlert(product.Reference, product.Quantity);
            logger.LogWarning("Stock alert generated for product {Reference}. Current stock: {Quantity}",
                product.Reference, product.Quantity);
        }
    }

    public async Task<Product> UpdateStockLevelAsync(int productId, int quantity, string operation)
    {
        var product = await productRepository.GetByIdAsync(productId)
                      ?? throw new NotFoundException($"Product {productId} not found");

        switch (operation.ToLower())
        {
            case "add":
                product.Quantity += quantity;
                break;
            case "remove":
                if (product.Quantity < quantity)
                    throw new ArgumentException($"Insufficient stock for product {product.Reference}");
                product.Quantity -= quantity;
                break;
            default:
                throw new ArgumentException($"Invalid operation: {operation}");
        }

        await productRepository.UpdateAsync(product);

        logger.LogInformation(
            "Stock level updated for product {Reference}: {Operation} {Quantity} units. New stock level: {NewQuantity}",
            product.Reference, operation, quantity, product.Quantity);

        return product;
    }

    public async Task ManageStockLevels(Product product)
    {
        var minStockLevel = product.MinimumStockLevel;
        if (product.Quantity <= minStockLevel) await CreateRestockAlert(product);
    }

    public async Task<Report> GenerateInventoryReport()
    {
        var products = await productRepository.GetAllAsync();
        var report = new Report
        {
            TotalProducts = products.Count(),
            AllProducts = products.ToList(),
            LowStockProducts = products.Where(p => p.Quantity <= p.MinimumStockLevel).ToList(),
            ExpiringProducts = products.Where(p => (p.ExpiryDate - DateTime.Today).Days <= 90).ToList(),
            TotalProductValue = products.Sum(p => p.Quantity * p.Price),
            OutOfStockCount = products.Count(p => p.Quantity == 0),
            StockAlerts = products.Where(p => p.Quantity <= p.MinimumStockLevel)
                .Select(p => new StockAlert(p))
                .ToList()
        };

        logger.LogInformation("Generated inventory report with {Count} products", products.Count());
        return report;
    }

    public async Task UpdateStock(int productId, int quantity, string operation)
    {
        var product = await productRepository.GetByIdAsync(productId)
                      ?? throw new NotFoundException($"Product {productId} not found");

        var stockOperation = Enum.Parse<StockOperation>(operation, true);
        product.UpdateStock(quantity, stockOperation);

        await productRepository.UpdateAsync(product);
    }

    public async Task<ExpiryStatus> CheckExpiryStatusAsync(int productId)
    {
        var product = await productRepository.GetByIdAsync(productId)
                      ?? throw new NotFoundException($"Product {productId} not found");

        var today = DateTime.UtcNow;
        var daysUntilExpiry = (product.ExpiryDate - today).Days;

        var status = daysUntilExpiry switch
                     {
                         < 0 => ExpiryStatus.Expired,
                         < 30 => ExpiryStatus.ExpiringIn1Month,
                         < 90 => ExpiryStatus.ExpiringIn3Months,
                         _ => ExpiryStatus.Valid
                     };

        if (status == ExpiryStatus.Valid) return status;
        await notificationService.SendExpiryAlert(
            product.Reference,
            product.BatchNumber,
            product.ExpiryDate,
            status);

        logger.LogWarning(
            "Product {Reference} (Batch: {Batch}) expiry status: {Status}. Expires on {Date}",
            product.Reference,
            product.BatchNumber,
            status,
            product.ExpiryDate);

        return status;
    }

    private async Task CreateRestockAlert(Product product)
    {
        var alert = new RestockAlert
        {
            ProductId = product.Id,
            ProductReference = product.Reference,
            CurrentStock = product.Quantity,
            MinimumStockLevel = product.MinimumStockLevel,
            CreatedAt = DateTime.UtcNow
        };

        await productRepository.AddAsync(alert);
        await notificationService.SendRestockAlert(alert);
        logger.LogInformation("Restock alert created for product {Reference}", product.Reference);
    }

    public async Task LoadInventory()
    {
        _state.SetState(s => s.IsLoading = true);

        try
        {
            var inventory = await _client.GetInventoryAsync();
            _state.SetState(s =>
            {
                s.Products = inventory.Products;
                s.LowStockCount = inventory.LowStockCount;
                s.ExpiringCount = inventory.ExpiringCount;
                s.IsLoading = false;
                s.Error = null;
            });
        }
        catch (Exception ex)
        {
            _state.SetState(s =>
            {
                s.IsLoading = false;
                s.Error = ex.Message;
            });
        }
    }
}