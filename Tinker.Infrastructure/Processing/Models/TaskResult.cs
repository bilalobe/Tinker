namespace Tinker.Infrastructure.Processing.Models;

public class TaskResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}