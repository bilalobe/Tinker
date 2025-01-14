namespace Tinker.Infrastructure.Core.Data.Interfaces;

/// <summary>
///     Provides generic CRUD operations for entities
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int                          id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken                           = default);
    Task<T> AddAsync(T                                 entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(T                           entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int                         id,     CancellationToken cancellationToken = default);
}