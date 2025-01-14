using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Security.Compliance.Models;
using Tinker.Shared.DTOs.Extras;
using Tinker.Shared.DTOs.Orders;

namespace Tinker.Infrastructure.Security.Compliance.Services;

public class ComplianceLoggingService(
    IApplicationDbContext             context,
    ILogger<ComplianceLoggingService> logger)
    : IComplianceLoggingService
{
    public Task LogCompliance(OrderDto order)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<ComplianceReport>> IComplianceLoggingService.GetComplianceReports(DateRange range)
    {
        throw new NotImplementedException();
    }

    public async Task LogCompliance(Order order)
    {
        var complianceLog = new ComplianceLog
        {
            OrderId = order.Id,
            Date = DateTime.UtcNow,
            CustomerId = order.CustomerId,
            Type = "OrderCompliance",
            Details = JsonSerializer.Serialize(new
            {
                order.Items,
                order.TotalAmount,
                order.Status
            })
        };

        context.ComplianceLogs.Add(complianceLog);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Compliance logged for order {OrderId}. Customer: {CustomerId}, Type: {Type}",
            order.Id, order.CustomerId, "OrderCompliance");
    }

    public async Task<IEnumerable<ComplianceReport>> GetComplianceReports(DateRange range)
    {
        var logs = await context.ComplianceLogs
            .Where(l => l.Date >= range.StartDate && l.Date <= range.EndDate)
            .ToListAsync();

        return Enumerable.Select <, ComplianceReport > (logs, l => new ComplianceReport
        {
            Id = l.OrderId,
            OrderDate = l.Date,
            Type = l.Type,
            Details = l.Details
        });
    }
}