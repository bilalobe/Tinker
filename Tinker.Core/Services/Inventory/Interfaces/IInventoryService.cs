namespace Tinker.Core.Services.Inventory.Interfaces;

public interface IInventoryService
{
    Task<bool> CheckStock(int                                      productId, int quantity);
    Task GenerateStockAlert(Product                                product);
    Task<Product> UpdateStockLevelAsync(int                        productId, int quantity, string operation);
    Task<InventoryService.ExpiryStatus> CheckExpiryStatusAsync(int productId);
    Task ManageStockLevels(Product                                 product);
    Task<Report> GenerateInventoryReport();
    Task UpdateStock(int productId, int quantity, string operation);
}