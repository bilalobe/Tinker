using Tinker.Shared.DTOs.Reports;

namespace Tinker.Infrastructure.Integration.Http.Clients.Interfaces;

public interface IReportingHttpClient
{
    Task<ReportDto> GetDashboardData();
}