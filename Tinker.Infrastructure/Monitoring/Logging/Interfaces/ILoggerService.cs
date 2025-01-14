using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Monitoring.Logging.Interfaces;

public interface ILoggerService
{
    void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, string? message,
        params object[]       args);

    void LogMetric(string      metricName, double value, Dictionary<string, object>? dimensions = null);
    void LogEvent(string       eventName,  Dictionary<string, object>? properties = null);
    void LogTrace(string       message,    params object[] args);
    void LogDebug(string       message,    params object[] args);
    void LogInformation(string message,    params object[] args);
    void LogWarning(string     message,    params object[] args);
    void LogError(Exception    exception,  string message, params object[] args);
    void LogCritical(Exception exception,  string message, params object[] args);
}