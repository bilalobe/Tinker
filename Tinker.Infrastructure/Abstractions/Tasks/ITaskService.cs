namespace Tinker.Infrastructure.Processing.Tasks.Interfaces;

public interface ITaskService
{
    Task RunScheduledTasks(CancellationToken stoppingToken);
}