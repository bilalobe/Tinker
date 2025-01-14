namespace Tinker.Infrastructure.Core.Data.Interfaces;

public interface IPaginationService
{
    Task<PaginatedResult<T>> CreatePaginatedResultAsync<T>(
        IQueryable<T>     query,
        int               pageNumber,
        int               pageSize,
        CancellationToken cancellationToken = default) where T : class;

    Task<PaginatedResult<TResult>> CreatePaginatedResultAsync<T, TResult>(
        IQueryable<T>     query,
        Func<T, TResult>  mapper,
        int               pageNumber,
        int               pageSize,
        CancellationToken cancellationToken = default) where T : class;
}