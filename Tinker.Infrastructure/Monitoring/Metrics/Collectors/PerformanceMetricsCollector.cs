using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Infrastructure.Monitoring.Metrics.Models;

namespace Tinker.Infrastructure.Monitoring.Metrics.Collectors;

public class PerformanceMetricsCollector(
    IMetricsService                      metricsService,
    ILogger<PerformanceMetricsCollector> logger)
    : BackgroundService
{
    private DateTime _lastCpuTime = DateTime.UtcNow;
    private TimeSpan _lastProcessorTime = TimeSpan.Zero;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                CollectMetrics();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error collecting performance metrics");
            }
    }

    private void CollectMetrics()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var currentTime = DateTime.UtcNow;
            var currentProcessorTime = process.TotalProcessorTime;

            // Calculate CPU usage percentage
            var cpuUsage = (currentProcessorTime - _lastProcessorTime).TotalSeconds /
                (currentTime - _lastCpuTime).TotalSeconds /
                Environment.ProcessorCount * 100;

            _lastCpuTime = currentTime;
            _lastProcessorTime = currentProcessorTime;

            // Memory in MB (both managed and unmanaged)
            var totalMemory = process.WorkingSet64 / (1024 * 1024);
            var managedMemory = GC.GetTotalMemory(false) / (1024 * 1024);

            // Record metrics
            metricsService.RecordGauge(PerformanceMetrics.CpuUsage, Math.Round(cpuUsage, 2));
            metricsService.RecordGauge(PerformanceMetrics.MemoryUsage, totalMemory);
            metricsService.RecordGauge(PerformanceMetrics.ManagedMemory, managedMemory);
            metricsService.RecordGauge(PerformanceMetrics.ThreadCount, process.Threads.Count);
            metricsService.RecordGauge(PerformanceMetrics.HandleCount, process.HandleCount);
            metricsService.RecordGauge(PerformanceMetrics.GcGen0, GC.CollectionCount(0));
            metricsService.RecordGauge(PerformanceMetrics.GcGen1, GC.CollectionCount(1));
            metricsService.RecordGauge(PerformanceMetrics.GcGen2, GC.CollectionCount(2));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error collecting performance metrics");
        }
    }
}