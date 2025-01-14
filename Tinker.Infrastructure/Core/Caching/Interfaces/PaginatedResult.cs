namespace Tinker.Infrastructure.Core.Data.Interfaces;

public record PaginatedResult<T>
{
    public required IEnumerable<T> Items { get; init; }

    public required int PageNumber { get; init; }

    public required int PageSize { get; init; }

    public required int TotalPages { get; init; }

    public required int TotalCount { get; init; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}