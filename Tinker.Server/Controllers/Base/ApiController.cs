using Microsoft.AspNetCore.Mvc;
using Tinker.Infrastructure.Core.Caching.Interfaces;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;

namespace Tinker.Server.Controllers.Base;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class ApiController(
    ILogger         logger,
    IMetricsService metrics,
    ICacheService   cache)
    : ControllerBase
{
    protected readonly ICacheService Cache = cache;
    protected readonly ILogger Logger = logger;
    protected readonly IMetricsService Metrics = metrics;

    protected async Task<ActionResult<T>> ExecuteWithMetrics<T>(
        Func<Task<T>> action,
        string        metricName,
        CacheOptions? cacheOptions = null)
    {
        using var timer = Metrics.StartTimer(metricName);
        try
        {
            if (cacheOptions != null)
            {
                var cacheKey = $"{metricName}:{Request.Path}";
                return Ok(await Cache.GetOrSetAsync(cacheKey, action, cacheOptions));
            }

            var result = await action();
            Metrics.IncrementCounter($"{metricName}.success");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing {MetricName}", metricName);
            Metrics.IncrementCounter($"{metricName}.error");
            return StatusCode(500, new ApiError("Internal server error", ex.Message));
        }
    }

    protected record ApiError(string Title, string Detail);
}