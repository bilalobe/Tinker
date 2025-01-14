using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Models;
using Tinker.Infrastructure.Monitoring.Metrics.Models;

namespace Tinker.Infrastructure.Monitoring.Metrics.Services;

public class MetricsService : IMetricsService, IDisposable
{
    private readonly ConcurrentDictionary<string, Timer> _callbacks;
    private readonly ILogger<MetricsService> _logger;
    private readonly Channel<MetricContext> _metricsChannel;
    private readonly ConcurrentDictionary<string, List<MetricContext>> _metricStore;
    private readonly TelemetryClient _telemetryClient;
    private bool _disposed;

    public MetricsService(ILogger<MetricsService> logger, TelemetryClient telemetryClient)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _callbacks = new ConcurrentDictionary<string, Timer>();
        _metricStore = new ConcurrentDictionary<string, List<MetricContext>>();

        var options = new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.DropWrite
        };
        _metricsChannel = Channel.CreateBounded<MetricContext>(options);

        _ = StartMetricProcessor();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            foreach (var timer in _callbacks.Values) timer.Dispose();
            _callbacks.Clear();
            _metricsChannel.Writer.Complete();
        }

        _disposed = true;
    }

    public void RecordMetric(MetricContext context)
    {
        if (!_metricsChannel.Writer.TryWrite(context))
            _logger.LogWarning("Metric channel full, dropped metric: {MetricName}", context.Name);
    }

    public void RecordMetric(string name, double value, MetricType type = MetricType.Counter, IEnumerable<MetricDimension>? dimensions = null)
    {
        var context = new MetricContext
        {
            Name = name,
            Value = value,
            Type = type,
            Dimensions = dimensions?.ToList() ?? new List<MetricDimension>(),
            Timestamp = DateTime.UtcNow
        };
        RecordMetric(context);
    }

    public void IncrementCounter(string name, double increment = 1, MetricDimension[]? metricDimensions = null)
    {
        var context = new MetricContext
        {
            Name = name,
            Value = increment,
            Type = MetricType.Counter,
            Dimensions = metricDimensions?.ToList() ?? new List<MetricDimension>(),
            Timestamp = DateTime.UtcNow
        };
        RecordMetric(context);
    }

    public void RecordGauge(string name, double value)
    {
        RecordMetric(name, value, MetricType.Gauge);
    }

    public void RecordHistogram(string name, double value)
    {
        RecordMetric(name, value, MetricType.Histogram);
    }

    public IDisposable StartTimer(string name, IEnumerable<MetricDimension>? dimensions = null)
    {
        var sw = Stopwatch.StartNew();
        return new TimerDisposable(() =>
        {
            sw.Stop();
            RecordMetric(name, sw.ElapsedMilliseconds, MetricType.Timer, dimensions);
        });
    }

    public void StopTimer(string name)
    {
        // Implementation for stopping a timer if needed
    }

    public IDictionary<string, double> GetCurrentMetrics()
    {
        return _metricStore
            .SelectMany(kvp => kvp.Value)
            .GroupBy(m => m.Name)
            .ToDictionary(
                g => g.Key,
                g => g.Average(m => m.Value));
    }

    public async Task<MetricsSnapshot> GetMetricsSnapshotAsync(DateTime? from = null, IEnumerable<MetricDimension>? filter = null)
    {
        var metrics = _metricStore
            .SelectMany(kvp => kvp.Value)
            .Where(m => !from.HasValue || m.Timestamp >= from.Value);

        if (filter != null)
        {
            var filterList = filter.ToList();
            metrics = metrics.Where(m =>
                filterList.All(f =>
                    m.Dimensions.Any(d => d.Name == f.Name && d.Value == f.Value)));
        }

        var snapshot = metrics
            .GroupBy(m => m.Name)
            .ToDictionary(
                g => g.Key,
                g => g.Average(m => m.Value));

        return await Task.FromResult(new MetricsSnapshot
        {
            Metrics = snapshot,
            Timestamp = DateTime.UtcNow
        });
    }

    public void RegisterCallback(string name, Func<double> callback, TimeSpan interval, MetricType type = MetricType.Gauge)
    {
        var timer = new Timer(_ =>
        {
            try
            {
                var value = callback();
                RecordMetric(name, value, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in metric callback for {MetricName}", name);
            }
        }, null, TimeSpan.Zero, interval);

        _callbacks.AddOrUpdate(name, timer, (_, existing) =>
        {
            existing.Dispose();
            return timer;
        });
    }

    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        await _metricsChannel.Writer.WaitToWriteAsync(cancellationToken);
    }

    public void Reset()
    {
        _metricStore.Clear();
    }

    private async Task StartMetricProcessor()
    {
        try
        {
            await foreach (var metric in _metricsChannel.Reader.ReadAllAsync())
            {
                await ProcessMetricAsync(metric);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing metrics");
        }
    }

    private async Task ProcessMetricAsync(MetricContext metric)
    {
        try
        {
            _metricStore.AddOrUpdate(
                metric.Name,
                [metric],
                (_, list) =>
                {
                    list.Add(metric);
                    return list;
                });

            var properties = metric.Dimensions
                .ToDictionary(d => d.Name, d => d.Value);

            _telemetryClient.TrackMetric(metric.Name, metric.Value, properties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing metric {MetricName}", metric.Name);
        }

        await Task.CompletedTask;
    }

    private sealed class TimerDisposable(Action onDispose) : IDisposable
    {
        private readonly Action _onDispose = onDispose;

        public void Dispose()
        {
            _onDispose();
        }
    }
}