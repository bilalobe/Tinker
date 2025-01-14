// Tinker.Infrastructure/Data/Interceptors/DbCommandInterceptor.cs

using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Models;
using Tinker.Infrastructure.Monitoring.Metrics;
using Tinker.Infrastructure.Monitoring.Metrics.Models;

namespace Tinker.Infrastructure.Core.Data.Interceptors;

public class PerformanceTrackingCommandInterceptor(IMetricsService metricsService) : DbCommandInterceptor
{
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand                        command,
        CommandEventData                 eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken                cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand         command,
        CommandEventData  eventData,
        DbDataReader      result,
        CancellationToken cancellationToken = default)
    {
        metricsService.RecordMetric(
            PerformanceMetrics.DatabaseQueryTime,
            eventData.Duration.TotalMilliseconds,
            MetricType.Timer,
            new[]
            {
                new MetricDimension("command_type", command.CommandType.ToString()),
                new MetricDimension("database", command.Connection?.Database ?? "unknown")
            });

        return new ValueTask<DbDataReader>(result);
    }
}