using System.Net;
using System.Net.Http.Headers;

namespace Tinker.Client.Infrastructure.Http.Handlers;

public class CacheHandler(ILocalStorageService localStorage, ILogger<CacheHandler> logger)
    : DelegatingHandler
{
    private static readonly Dictionary<string, CacheEntry> MemoryCache = new();
    private readonly ILocalStorageService _localStorage = localStorage;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken  cancellationToken)
    {
        if (request.Method != HttpMethod.Get)
            return await base.SendAsync(request, cancellationToken);

        var cacheKey = GetCacheKey(request);

        try
        {
            // Check memory cache first
            if (MemoryCache.TryGetValue(cacheKey, out var memoryEntry) && !IsExpired(memoryEntry))
            {
                logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return CreateResponseFromCache(memoryEntry);
            }

            // Check local storage
            var storageEntry = await _localStorage.GetItemAsync<CacheEntry>(cacheKey);
            if (storageEntry != null && !IsExpired(storageEntry))
            {
                logger.LogInformation("Storage hit for {CacheKey}", cacheKey);
                MemoryCache[cacheKey] = storageEntry;
                return CreateResponseFromCache(storageEntry);
            }

            // If not in cache, make the request
            var response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode) await CacheResponse(cacheKey, response);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling cached request for {CacheKey}", cacheKey);
            throw;
        }
    }

    private static string GetCacheKey(HttpRequestMessage request)
    {
        return $"{request.Method}:{request.RequestUri}";
    }

    private static bool IsExpired(CacheEntry entry)
    {
        return entry.ExpiresAt < DateTime.UtcNow;
    }

    private async Task CacheResponse(string key, HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var entry = new CacheEntry
        {
            Content = content,
            ContentType = response.Content.Headers.ContentType?.ToString(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        MemoryCache[key] = entry;
        await _localStorage.SetItemAsync(key, entry);
    }

    private static HttpResponseMessage CreateResponseFromCache(CacheEntry entry)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(entry.Content)
        };

        if (entry.ContentType != null)
            response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(entry.ContentType);

        return response;
    }

    private class CacheEntry
    {
        public string Content { get; set; } = "";
        public string? ContentType { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}