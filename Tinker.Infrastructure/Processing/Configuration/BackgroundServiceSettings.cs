namespace Tinker.Infrastructure.Processing.Configuration;

public class BackgroundServiceSettings
{
    public int StockCheckIntervalMinutes { get; set; } = 60;
    public int ExpiryCheckIntervalMinutes { get; set; } = 60;
    public int ExpiryWarningDays { get; set; } = 30;
}