using Tinker.Core.Domain.Compliance.Entities;

namespace Tinker.Core.Domain.Compliance.Repositories;

public interface IComplianceRepository
{
    Task<ComplianceLog?> GetByIdAsync(int id);
    Task<IEnumerable<ComplianceLog>> GetAllAsync();
    Task AddAsync(ComplianceLog    complianceLog);
    Task UpdateAsync(ComplianceLog complianceLog);
    Task DeleteAsync(int           id);
}