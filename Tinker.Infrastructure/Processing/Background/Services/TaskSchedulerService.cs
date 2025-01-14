using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Processing.Tasks.Interfaces;

namespace Tinker.Infrastructure.Processing.Background.Services;

public class TaskSchedulerService(ILogger<TaskSchedulerService> logger, IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("TaskSchedulerService running at: {time}", DateTimeOffset.Now);
            await ScheduleTasks(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ScheduleTasks(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

        try
        {
            await taskService.RunScheduledTasks(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running scheduled tasks");
        }
    }
}