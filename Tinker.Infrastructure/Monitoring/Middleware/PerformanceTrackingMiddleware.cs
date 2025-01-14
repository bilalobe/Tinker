using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Models;
using Tinker.Infrastructure.Monitoring.Metrics;
using Tinker.Infrastructure.Monitoring.Metrics.Models;

namespace Tinker.Infrastructure.Monitoring.Middleware;

public class PerformanceTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetricsService _metricsService;
    private readonly ILogger _logger;

    public PerformanceTrackingMiddleware(
        RequestDelegate next,
        IMetricsService metricsService,
        ILogger<PerformanceTrackingMiddleware> logger)
    {
        _next = next;
        _metricsService = metricsService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        try
        {
            _metricsService.IncrementCounter(
                PerformanceMetrics.HttpRequestRate,
                1.0,
                [new MetricDimension("path", path)]);

            await _next(context);

            sw.Stop();
            
            _metricsService.RecordMetric(
                PerformanceMetrics.ResponseTime,
                sw.ElapsedMilliseconds,
                MetricType.Timer,
                [
                    new MetricDimension("path", path),
                    new MetricDimension("status_code", context.Response.StatusCode.ToString())
                ]);
        }
        catch (Exception ex)
        {
            _metricsService.IncrementCounter(
                PerformanceMetrics.HttpErrorRate,
                1.0,
                [new MetricDimension("path", path)]);
                
            _logger.LogError(ex, "Request failed for {Path}", path);
            throw;
        }
    }
}