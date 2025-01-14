namespace Tinker.Shared.Exceptions;

public record ExceptionDetails
{
    public required string Message { get; init; }
    public required string ErrorCode { get; init; }
    public string? ErrorType { get; init; }
    public DateTime Timestamp { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();
}