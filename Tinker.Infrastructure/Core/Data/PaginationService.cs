
using Microsoft.EntityFrameworkCore;
using Tinker.Infrastructure.Core.Data.Interfaces;

namespace Tinker.Infrastructure.Core.Data;

public class PaginationService : IPaginationService
{
    public async Task<PaginatedResult<T>> CreatePaginatedResultAsync<T>(
        IQueryable<T>     query,
        int               pageNumber,
        int               pageSize,
        CancellationToken cancellationToken = default) where T : class
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}