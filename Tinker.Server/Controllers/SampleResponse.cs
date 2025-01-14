namespace Tinker.Server.Controllers;

/// <summary>
///     Represents a sample response from the API
/// </summary>
public class SampleResponse
{
    /// <summary>
    ///     The response message
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    ///     The timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; init; }
}