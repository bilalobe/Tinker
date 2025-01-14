using Tinker.Core.Domain.Compliance.Entities;

namespace Tinker.Core.Services.Compliance.Interfaces;

public interface IComplianceService
{
    Task CreateComplianceLog(ComplianceLog complianceLog);
}