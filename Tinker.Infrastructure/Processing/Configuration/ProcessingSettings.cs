namespace Tinker.Infrastructure.Processing.Configuration;

public class ProcessingSettings
{
    public int TaskExecutionIntervalMinutes { get; set; } = 60;
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelaySeconds { get; set; } = 5;
}