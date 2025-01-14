namespace Tinker.Infrastructure.Monitoring.Health.Models;

public record HealthCheckOptions
{
    public long MemoryThresholdBytes { get; init; } = 1024L * 1024L * 1024L; // 1GB
    public int MaxThreadCount { get; init; } = 200;
    public int MaxHandleCount { get; init; } = 1000;
    public double MinDiskSpacePercent { get; init; } = 10;
    public TimeSpan DatabaseTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public int MaxFailedAttempts { get; init; } = 3;
    public TimeSpan CircuitBreakerDuration { get; init; } = TimeSpan.FromMinutes(1);
}