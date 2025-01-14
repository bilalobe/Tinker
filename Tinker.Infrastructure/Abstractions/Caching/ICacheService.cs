namespace Tinker.Infrastructure.Core.Caching.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task<bool> SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    Task<bool> SetManyAsync<T>(IDictionary<string, T> keyValues, CacheOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheOptions? options = null, CancellationToken cancellationToken = default);

    Task<bool> UpdateExpiryAsync(string key, TimeSpan newExpiry, CancellationToken cancellationToken = default);

    Task<bool> AddToTagAsync(string tag, string key, CancellationToken cancellationToken = default);
    Task<bool> InvalidateByTagAsync(string tag, CancellationToken cancellationToken = default);

    // New methods
    Task<bool> SetWithTagsAsync<T>(string key, T value, string[] tags, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetTagKeysAsync(string tag, CancellationToken cancellationToken = default);
    Task<bool> LockAsync(string key, TimeSpan duration, CancellationToken cancellationToken = default);
    Task<bool> UnlockAsync(string key, CancellationToken cancellationToken = default);
}