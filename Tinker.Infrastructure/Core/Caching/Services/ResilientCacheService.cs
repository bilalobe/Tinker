using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tinker.Infrastructure.Core.Caching.Interfaces;

namespace Tinker.Infrastructure.Core.Caching.Services;

public class ResilientCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<ResilientCacheService> _logger;
    private readonly CacheConfiguration _config;
    private readonly Polly.ResiliencePipeline _pipeline;

    public ResilientCacheService(
        IConnectionMultiplexer redis,
        ILogger<ResilientCacheService> logger,
        CacheConfiguration config)
    {
        _redis = redis;
        _logger = logger;
        _config = config;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
        }, cancellationToken);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var serialized = JsonSerializer.Serialize(value);
            await db.StringSetAsync(
                key,
                serialized,
                ttl ?? _config.DefaultTtl);
            return true;
        }, cancellationToken);
    }

    public async Task InvalidateAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }, cancellationToken);
    }

    public async Task InvalidatePatternAsync(
        string pattern,
        CancellationToken cancellationToken = default)
    {
        await _pipeline.ExecuteAsync(async ct =>
        {
            var keys = await GetKeysByPatternAsync(pattern);
            var db = _redis.GetDatabase();
            var tasks = keys.Select(key => db.KeyDeleteAsync(key));
            await Task.WhenAll(tasks);
            return true;
        }, cancellationToken);
    }

    private async Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
    {
        var keys = new List<string>();
        foreach (var endpoint in _redis.GetEndPoints())
        {
            var server = _redis.GetServer(endpoint);
            var serverKeys = server.Keys(pattern: pattern);
            keys.AddRange(serverKeys.Select(k => k.ToString()));
        }
        return keys;
    }

    public async Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var batch = db.CreateBatch();
            var tasks = keys.Select(key => batch.StringGetAsync(key)).ToList();
            batch.Execute();
            await Task.WhenAll(tasks);
            
            return keys.Zip(tasks, (key, task) => new
            {
                Key = key,
                Value = task.Result.HasValue ? JsonSerializer.Deserialize<T>(task.Result!) : default
            }).ToDictionary(x => x.Key, x => x.Value);
        }, cancellationToken);
    }

    public async Task<bool> AddToTagAsync(string tag, string key, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var tagKey = $"tag:{tag}";
            return await db.SetAddAsync(tagKey, key);
        }, cancellationToken);
    }

    public async Task<bool> InvalidateByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var tagKey = $"tag:{tag}";
            var members = await db.SetMembersAsync(tagKey);
            if (!members.Any()) return true;

            var batch = db.CreateBatch();
            var tasks = members.Select(m => batch.KeyDeleteAsync(m.ToString())).ToList();
            tasks.Add(batch.KeyDeleteAsync(tagKey));
            batch.Execute();
            await Task.WhenAll(tasks);
            return true;
        }, cancellationToken);
    }

    public async Task<bool> SetAsync<T>(string key, T value, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var serialized = JsonSerializer.Serialize(value);
            await db.StringSetAsync(
                key,
                serialized,
                options?.Ttl ?? _config.DefaultTtl);
            return true;
        }, cancellationToken);
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }, cancellationToken);
    }

    public async Task<bool> SetManyAsync<T>(IDictionary<string, T> keyValues, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var batch = db.CreateBatch();
            var tasks = keyValues.Select(kv =>
            {
                var serialized = JsonSerializer.Serialize(kv.Value);
                return batch.StringSetAsync(kv.Key, serialized, options?.Ttl ?? _config.DefaultTtl);
            }).ToList();
            batch.Execute();
            await Task.WhenAll(tasks);
            return true;
        }, cancellationToken);
    }

    public async Task<bool> RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var batch = db.CreateBatch();
            var tasks = keys.Select(key => batch.KeyDeleteAsync(key)).ToList();
            batch.Execute();
            await Task.WhenAll(tasks);
            return true;
        }, cancellationToken);
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, CacheOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value!);
            }

            var result = await factory();
            var serialized = JsonSerializer.Serialize(result);
            await db.StringSetAsync(key, serialized, options?.Ttl ?? _config.DefaultTtl);
            return result;
        }, cancellationToken);
    }

    public async Task<bool> UpdateExpiryAsync(string key, TimeSpan newExpiry, CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(async ct =>
        {
            var db = _redis.GetDatabase();
            return await db.KeyExpireAsync(key, newExpiry);
        }, cancellationToken);
    }
}