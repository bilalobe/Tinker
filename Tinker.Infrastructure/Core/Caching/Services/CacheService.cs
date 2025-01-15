using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Core.Caching.Compression;
using Tinker.Infrastructure.Core.Caching.Interfaces;

namespace Tinker.Infrastructure.Core.Caching.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ICompressionService _compressionService;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ICacheMetrics _metrics;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        IDistributedCache cache,
        ILogger<CacheService> logger,
        ICacheMetrics metrics,
        ICompressionService compressionService)
    {
        _cache = cache;
        _logger = logger;
        _metrics = metrics;
        _compressionService = compressionService;

        _retryPolicy = Microsoft.AspNetCore.Authorization.Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception,
                        "Retry {RetryCount} after {Delay}ms for operation {OperationKey}",
                        retryCount, timeSpan.TotalMilliseconds, context.OperationKey);
                });
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var value = await _cache.GetAsync(key, cancellationToken);
                if (value == null)
                {
                    _metrics.IncrementMisses("default");
                    return default;
                }

                _metrics.IncrementHits("default");
                _metrics.TrackSize("default", value.Length);

                var result = await DeserializeWithCompressionAsync<T>(value);
                _metrics.TrackOperationDuration("get", DateTime.UtcNow - startTime);
                return result;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
            return default;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = options?.Expiry ?? TimeSpan.FromMinutes(30),
                SlidingExpiration = options?.SlidingExpiration
            };

            var serializedValue = JsonSerializer.Serialize(value);

            if (options?.UseCompression == true)
            {
                var compressedData = await CompressData(serializedValue);
                if (compressedData != null) serializedValue = "COMPRESSED:" + Convert.ToBase64String(compressedData);
            }

            await _cache.SetStringAsync(key, serializedValue, cacheOptions, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetAsync(key, cancellationToken);
            return value != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key: {Key}", key);
            return false;
        }
    }

    public async Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var tasks = keys.Select(key => GetAsync<T>(key, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return keys.Zip(results, (k, v) => (k, v)).ToDictionary(x => x.k, x => x.v);
    }

    public async Task<bool> SetManyAsync<T>(IDictionary<string, T> keyValues, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var tasks = keyValues.Select(kv => SetAsync(kv.Key, kv.Value, options, cancellationToken));
            var results = await Task.WhenAll(tasks);
            return results.All(x => x);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting multiple cache values");
            return false;
        }
    }

    public async Task<bool> RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            var tasks = keys.Select(key => RemoveAsync(key, cancellationToken));
            var results = await Task.WhenAll(tasks);
            return results.All(x => x);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing multiple cache values");
            return false;
        }
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<T>(key, cancellationToken);
        if (!object.Equals(value, default(T))) return value;

        var newValue = await factory();
        await SetAsync(key, newValue, options, cancellationToken);
        return newValue;
    }

    public async Task<bool> UpdateExpiryAsync(string key, TimeSpan newExpiry, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _cache.GetAsync(key, cancellationToken);
            if (value == null) return false;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = newExpiry
            };

            await _cache.SetAsync(key, value, options, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expiry for key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> AddToTagAsync(string tag, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var tagKey = $"tag:{tag}";
            var keys = await GetAsync<HashSet<string>>(tagKey, cancellationToken) ?? new HashSet<string>();
            keys.Add(key);
            await SetAsync(tagKey, keys, null, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding key to tag: {Tag}", tag);
            return false;
        }
    }

    public async Task<bool> InvalidateByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        try
        {
            var tagKey = $"tag:{tag}";
            var keys = await GetAsync<HashSet<string>>(tagKey, cancellationToken) ?? new HashSet<string>();
            await RemoveManyAsync(keys, cancellationToken);
            await RemoveAsync(tagKey, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache by tag: {Tag}", tag);
            return false;
        }
    }

    private static async Task<byte[]?> CompressData(string data)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        using (var writer = new StreamWriter(gzipStream))
        {
            await writer.WriteAsync(data);
        }

        return memoryStream.ToArray();
    }

    private static async Task<string?> DecompressData(byte[] compressedData)
    {
        using var memoryStream = new MemoryStream(compressedData);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream);
        return await reader.ReadToEndAsync();
    }

    private async Task<T?> DeserializeWithCompressionAsync<T>(byte[] data)
    {
        try
        {
            var isCompressed = data.Length >= 2 && data[0] == 0x1f && data[1] == 0x8b;
            var stringData = isCompressed
                ? await _compressionService.DecompressAsync(data)
                : Encoding.UTF8.GetString(data);

            return stringData == null ? default : JsonSerializer.Deserialize<T>(stringData);
        }
        catch
        {
            return default;
        }
    }
}