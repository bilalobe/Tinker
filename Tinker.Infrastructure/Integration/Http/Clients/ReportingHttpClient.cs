using System.Net.Http.Json;
using Tinker.Infrastructure.Integration.Http.Clients.Interfaces;
using Tinker.Shared.DTOs.Reports;

namespace Tinker.Infrastructure.Integration.Http.Clients;

public abstract class ReportingHttpClient(HttpClient client) : IReportingHttpClient
{
    public async Task<ReportDto> GetDashboardData()
    {
        return await client.GetFromJsonAsync<ReportDto>("/api/reports/dashboard")
               ?? throw new Exception("Failed to load dashboard data");
    }
}