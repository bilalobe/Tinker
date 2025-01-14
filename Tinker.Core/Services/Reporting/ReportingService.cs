using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Customers.Repositories;
using Tinker.Core.Domain.Inventory.Events;
using Tinker.Core.Domain.Orders.Repositories;
using Tinker.Core.Domain.Products.Repositories;
using Tinker.Core.Services.Compliance.Interfaces;
using Tinker.Core.Services.Reporting.Interface;
using Tinker.Shared.DTOs.Customers;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Reporting;

public class ReportingService(
    IOrderRepository          orderRepository,
    IProductRepository        productRepository,
    ICustomerRepository       customerRepository,
    IComplianceService        complianceService,
    ILogger<ReportingService> logger)
    : IReportingService
{
    private readonly IComplianceService _complianceService = complianceService;

    public async Task<Report> GenerateExpiryReport()
    {
        // Implementation for GenerateExpiryReport
        return await Task.FromResult(new Report());
    }

    public async Task<Report> GenerateRxComplianceReport()
    {
        var orders = await orderRepository.GetRxOrdersAsync();

        logger.LogInformation("Generated Rx compliance report with {Count} orders", orders.Count);

        return new Report
        {
            RxOrders = orders,
            ComplianceDate = DateTime.UtcNow
        };
    }

    public async Task<Report> GenerateSalesReport(DateTime startDate, DateTime endDate)
    {
        var orders = await orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);

        return new Report
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalSales = orders.Sum(o => o.TotalAmount),
            TotalOrders = orders.Count,
            AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0,
            DailySales = GetDailySales(orders)
        };
    }

    public async Task<Report> GenerateInventoryReport()
    {
        var products = await productRepository.GetAllAsync();
        return new Report
        {
            LowStockProducts = products.Where(p => p.Quantity <= p.MinimumStockLevel).ToList(),
            TotalProductValue = products.Sum(p => p.Quantity * p.Price),
            OutOfStockCount = products.Count(p => p.Quantity == 0),
            StockAlerts = products.Where(p => p.Quantity <= p.MinimumStockLevel)
                .Select(p => new StockAlert(p)).ToList()
        };
    }

    public async Task<Report> GenerateCustomerReport()
    {
        var customers = await customerRepository.GetAllWithOrdersAsync();

        return new Report
        {
            TotalCustomers = customers.Count,
            ActiveCustomers = customers.Count(c => c.LastPurchaseDate >= DateTime.UtcNow.AddMonths(-3)),
            TopCustomers = customers
                .OrderByDescending(c => c.Orders.Sum(o => o.TotalAmount))
                .Take(10)
                .ToList(),
            LoyaltyTierDistribution = customers.GroupBy(c => c.MembershipTier)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<CustomerStatistics> GetCustomerStatistics(int customerId)
    {
        var customer = await customerRepository.GetByIdWithOrdersAsync(customerId)
                       ?? throw new NotFoundException($"Customer {customerId} not found");

        var orders = customer.Orders.ToList();

        return new CustomerStatistics
        {
            TotalOrders = orders.Count,
            TotalSpent = orders.Sum(o => o.TotalAmount),
            LoyaltyPoints = customer.LoyaltyPoints,
            MembershipTier = customer.MembershipTier,
            LastPurchaseDate = orders.Max(o => o.Date)
        };
    }

    public async Task<Report> GenerateStockLevelReport()
    {
        var products = await productRepository.GetAllAsync();
        return new Report
        {
            TotalProducts = products.Count,
            AllProducts = products
        };
    }

    private static Dictionary<DateTime, decimal> GetDailySales(List<Order> orders)
    {
        return orders.GroupBy(o => o.Date.Date)
            .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount));
    }
}