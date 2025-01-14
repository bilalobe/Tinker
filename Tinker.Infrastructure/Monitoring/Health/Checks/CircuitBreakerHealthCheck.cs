using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Monitoring.Health.Models;

namespace Tinker.Infrastructure.Monitoring.Health.Checks;

public class CircuitBreakerHealthCheck(
    IHealthCheck                       innerCheck,
    HealthCheckOptions                 options,
    ILogger<CircuitBreakerHealthCheck> logger)
    : IHealthCheck
{
    private readonly object _lock = new();

    private int _failedAttempts;
    private DateTime _lastFailure = DateTime.MinValue;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken  cancellationToken = default)
    {
        if (IsCircuitOpen())
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "Circuit breaker is open");

        try
        {
            var result = await innerCheck.CheckHealthAsync(context, cancellationToken);
            if (result.Status == HealthStatus.Healthy)
                Reset();
            else
                IncrementFailure();
            return result;
        }
        catch (Exception ex)
        {
            IncrementFailure();
            logger.LogError(ex, "Health check failed");
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }

    private bool IsCircuitOpen()
    {
        lock (_lock)
        {
            if (_failedAttempts >= options.MaxFailedAttempts)
            {
                var timeSinceLastFailure = DateTime.UtcNow - _lastFailure;
                if (timeSinceLastFailure <= options.CircuitBreakerDuration) return true;
                Reset();
            }

            return false;
        }
    }

    private void IncrementFailure()
    {
        lock (_lock)
        {
            _failedAttempts++;
            _lastFailure = DateTime.UtcNow;
        }
    }

    private void Reset()
    {
        lock (_lock)
        {
            _failedAttempts = 0;
            _lastFailure = DateTime.MinValue;
        }
    }
}