namespace Tinker.Infrastructure.Core.Caching.Interfaces;

public record CacheOptions
{
    public TimeSpan? Expiry { get; init; }
    public TimeSpan? SlidingExpiration { get; init; }
    public bool UseCompression { get; init; }
    public int CompressionThreshold { get; init; } = 1024;
    public CachePriority Priority { get; init; } = CachePriority.Normal;
    public string Region { get; init; } = "default";
    public ISet<string> Tags { get; init; } = new HashSet<string>();
    public TimeSpan Ttl { get; internal set; }
}