namespace Tinker.Shared.Exceptions;

public abstract class BusinessException : Exception
{
    protected BusinessException(
        string                      message,
        string                      errorCode = "UNKNOWN_ERROR",
        Dictionary<string, object>? metadata  = null,
        string?                     errorType = null) : base(message)
    {
        ErrorCode = errorCode;
        Metadata = metadata ?? new Dictionary<string, object>();
        ErrorType = errorType ?? GetType().Name;
        Timestamp = DateTime.UtcNow;
    }

    public string ErrorCode { get; }
    public Dictionary<string, object> Metadata { get; }
    public string? ErrorType { get; }
    public DateTime Timestamp { get; }

    public virtual ExceptionDetails GetDetails()
    {
        return new ExceptionDetails
        {
            Message = Message,
            ErrorCode = ErrorCode,
            ErrorType = ErrorType,
            Timestamp = Timestamp,
            Metadata = Metadata
        };
    }
}