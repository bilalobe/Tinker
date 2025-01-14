using Microsoft.EntityFrameworkCore;
using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Infrastructure.Core.Data.Repositories.Compliance;

public class ComplianceRepository(ApplicationDbContext context) : IComplianceRepository
{
    public async Task<ComplianceLog?> GetByIdAsync(int id)
    {
        return await context.ComplianceLogs.FindAsync(id);
    }

    public async Task<IEnumerable<ComplianceLog>> GetAllAsync()
    {
        return await context.ComplianceLogs.ToListAsync();
    }

    public async Task AddAsync(ComplianceLog complianceLog)
    {
        await context.ComplianceLogs.AddAsync(complianceLog);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ComplianceLog complianceLog)
    {
        context.ComplianceLogs.Update(complianceLog);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var complianceLog = await GetByIdAsync(id);
        if (complianceLog != null)
        {
            context.ComplianceLogs.Remove(complianceLog);
            await context.SaveChangesAsync();
        }
    }
}