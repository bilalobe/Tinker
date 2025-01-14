namespace Tinker.Infrastructure.Integration.Http.Interfaces;

public interface IResiliencePipeline
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action);
}