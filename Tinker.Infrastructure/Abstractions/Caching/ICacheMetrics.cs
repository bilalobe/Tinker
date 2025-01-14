namespace Tinker.Infrastructure.Core.Caching.Interfaces;

public interface ICacheMetrics
{
    void IncrementHits(string          region);
    void IncrementMisses(string        region);
    void TrackSize(string              region,    long     bytes);
    void TrackOperationDuration(string operation, TimeSpan duration);
}