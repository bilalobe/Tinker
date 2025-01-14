using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Monitoring.Logging.Interfaces;
using ILogger = Serilog.ILogger;

namespace Tinker.Infrastructure.Monitoring.Logging.Services;

public class LoggerService(ILogger<LoggerService> logger, TelemetryClient telemetryClient)
    : ILoggerService
{
    private readonly ILogger _logger = logger;
    private readonly Microsoft.Identity.Client.TelemetryCore.TelemetryClient _telemetryClient = telemetryClient;

    public void Log<TState>(
        LogLevel        logLevel,
        EventId         eventId,
        TState          state,
        Exception?      exception,
        string?         message,
        params object[] args)
    {
        _logger.Log(logLevel, eventId, state, exception, message, args);
    }

    public void LogMetric(string metricName, double value, Dictionary<string, object>? dimensions = null)
    {
        _telemetryClient.TrackMetric(metricName, value, dimensions?.ToDictionary(x => x.Key, x => x.Value.ToString()));
        _logger.LogInformation("Metric: {MetricName} = {Value} {Dimensions}",
            metricName, value, dimensions ?? new Dictionary<string, object>());
    }

    public void LogEvent(string eventName, Dictionary<string, object>? properties = null)
    {
        _telemetryClient.TrackEvent(eventName, properties?.ToDictionary(x => x.Key, x => x.Value.ToString()));
        _logger.LogInformation("Event: {EventName} {Properties}",
            eventName, properties ?? new Dictionary<string, object>());
    }

    public void LogTrace(string message, params object[] args)
    {
        _logger.LogTrace(message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
        _telemetryClient.TrackException(exception, new Dictionary<string, string>
        {
            { "message", string.Format(message, args) }
        });
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        _logger.LogCritical(exception, message, args);
        _telemetryClient.TrackException(exception, new Dictionary<string, string>
        {
            { "message", string.Format(message, args) },
            { "severity", "Critical" }
        });
    }
}