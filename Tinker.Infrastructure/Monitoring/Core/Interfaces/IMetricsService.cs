using Tinker.Infrastructure.Monitoring.Core.Models;
using Tinker.Infrastructure.Monitoring.Metrics;
using Tinker.Infrastructure.Monitoring.Metrics.Models;

namespace Tinker.Infrastructure.Monitoring.Core.Interfaces;

public interface IMetricsService
{
    void RecordMetric(MetricContext context);

    void RecordMetric(string name, double value, MetricType type = MetricType.Counter, IEnumerable<MetricDimension>? dimensions = null);

    void IncrementCounter(string name, double increment = 1, MetricDimension[]? metricDimensions = null);
    void RecordGauge(string name, double value);
    void RecordHistogram(string name, double value);
    IDisposable StartTimer(string name, IEnumerable<MetricDimension>? dimensions = null);
    void StopTimer(string name);
    IDictionary<string, double> GetCurrentMetrics();

    Task<MetricsSnapshot> GetMetricsSnapshotAsync(DateTime? from = null, IEnumerable<MetricDimension>? filter = null);

    void RegisterCallback(string name, Func<double> callback, TimeSpan interval, MetricType type = MetricType.Gauge);

    Task FlushAsync(CancellationToken cancellationToken = default);
    void Reset();
}