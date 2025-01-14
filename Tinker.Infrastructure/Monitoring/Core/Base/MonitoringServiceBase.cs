using Tinker.Infrastructure.Monitoring.Core.Interfaces;

namespace Tinker.Infrastructure.Monitoring.Core.Base;

public abstract class MonitoringServiceBase
{
    protected readonly ILogger Logger;
    protected readonly IMetricsService MetricsService;

    protected MonitoringServiceBase(
        ILogger logger,
        IMetricsService metricsService)
    {
        Logger = logger;
        MetricsService = metricsService;
    }

    protected abstract Task CollectMetricsAsync();
    protected abstract Task ValidateHealthAsync();
}