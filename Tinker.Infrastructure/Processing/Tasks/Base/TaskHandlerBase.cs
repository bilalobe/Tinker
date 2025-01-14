using Tinker.Infrastructure.Core.Data.Interfaces;

namespace Tinker.Infrastructure.Processing.Tasks.Base;

public abstract class TaskHandlerBase
{
    protected readonly ILogger Logger;
    protected readonly IApplicationDbContext Context;

    protected TaskHandlerBase(
        ILogger logger,
        IApplicationDbContext context)
    {
        Logger = logger;
        Context = context;
    }

    protected async Task ExecuteWithRetry(Func<Task> action, int maxRetries = 3)
    {
        var attempt = 0;
        while (attempt < maxRetries)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex)
            {
                attempt++;
                Logger.LogWarning(ex, "Retry attempt {Attempt} of {MaxRetries}", 
                    attempt, maxRetries);
                
                if (attempt == maxRetries) throw;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
        }
    }
}