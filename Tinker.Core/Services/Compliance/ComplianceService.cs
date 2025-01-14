using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Compliance.Entities;
using Tinker.Core.Domain.Compliance.Repositories;
using Tinker.Core.Services.Compliance.Interfaces;

namespace Tinker.Core.Services.Compliance;

public class ComplianceService(IComplianceRepository complianceRepository, ILogger<ComplianceService> logger)
    : IComplianceService
{
    public async Task CreateComplianceLog(ComplianceLog complianceLog)
    {
        await complianceRepository.AddAsync(complianceLog);
        logger.LogInformation("Compliance log {ComplianceLogId} created successfully", complianceLog.Id);
    }
}