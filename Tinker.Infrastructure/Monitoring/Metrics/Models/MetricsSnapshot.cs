namespace Tinker.Infrastructure.Monitoring.Metrics.Models;

public record MetricsSnapshot
{
    public required Dictionary<string, double> Metrics { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}