namespace Tinker.Shared.Models.ErrorHandling;

public record ErrorContext
{
    public required string Component { get; init; }
    public required string Location { get; init; }
    public required DateTime Timestamp { get; init; }
    public IDictionary<string, string> Data { get; init; } = new Dictionary<string, string>();
    public int RetryCount { get; init; }
    public string? StackTrace { get; init; }
}