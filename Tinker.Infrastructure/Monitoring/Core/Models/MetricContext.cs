using Tinker.Infrastructure.Monitoring.Metrics;

namespace Tinker.Infrastructure.Monitoring.Core.Models;

public record MetricContext
{
    public string Name { get; init; } = string.Empty;
    public MetricType Type { get; init; }
    public double Value { get; init; }
    public IReadOnlyCollection<MetricDimension> Dimensions { get; init; } = Array.Empty<MetricDimension>();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}