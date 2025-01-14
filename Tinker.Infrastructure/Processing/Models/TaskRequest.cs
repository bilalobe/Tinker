namespace Tinker.Infrastructure.Processing.Models;

public class TaskRequest
{
    public string TaskName { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}