using Tinker.Shared.DTOs.Customers;

namespace Tinker.Core.Services.Reporting.Interface;

public interface IReportingService
{
    Task<Report> GenerateExpiryReport();
    Task<Report> GenerateStockLevelReport();
    Task<Report> GenerateRxComplianceReport();
    Task<Report> GenerateSalesReport(DateTime startDate, DateTime endDate);
    Task<Report> GenerateInventoryReport();
    Task<Report> GenerateCustomerReport();
    Task<CustomerStatistics> GetCustomerStatistics(int customerId);
}